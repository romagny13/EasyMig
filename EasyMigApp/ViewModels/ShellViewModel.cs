using EasyMigApp.Models;
using EasyMigLib.MigrationReflection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasyMigApp.ViewModels
{
    public class ShellViewModel : Observable
    {
        private IMigrationService migrationService;

        private MigrationModel model;
        public MigrationModel Model
        {
            get { return model; }
            set { this.Set(ref model, value); }
        }

        private ObservableCollection<RecognizedMigrationFile> migrations;
        public ObservableCollection<RecognizedMigrationFile> Migrations
        {
            get { return migrations; }
            set { this.Set(ref migrations, value); }
        }

        private ObservableCollection<RecognizedMigrationFile> seeders;
        public ObservableCollection<RecognizedMigrationFile> Seeders
        {
            get { return seeders; }
            set { this.Set(ref seeders, value); }
        }

        private Visibility isActive;
        public Visibility IsActive
        {
            get { return isActive; }
            set { this.Set(ref isActive, value); }
        }

        public bool HasMigrations => this.Migrations.Count > 0;
        public bool HasSeeders => this.Seeders.Count > 0;

        public List<string> ProviderNames { get; set; }

        public List<string> StoredConnectionStrings { get; set; }

        public ICommand AssemblyPathCommand { get; set; }

        public ICommand UpAllCommand { get; set; }
        public ICommand DownAllCommand { get; set; }
        public ICommand UpOneCommand { get; set; }
        public ICommand DownOneCommand { get; set; }

        public ICommand SeedAllCommand { get; set; }
        public ICommand SeedOneCommand { get; set; }

        public ICommand GenMigrationScriptCommand { get; set; }
        public ICommand GenSeedScriptCommand { get; set; }

        public ShellViewModel()
            : this(new MigrationService())
        { }

        public ShellViewModel(IMigrationService migrationService)
        {
            this.SetInactive();

            this.migrationService = migrationService;

            this.Migrations = new ObservableCollection<RecognizedMigrationFile>();
            this.seeders = new ObservableCollection<RecognizedMigrationFile>();

            this.RestoreConnectionStrings();

            this.ProviderNames = new List<string>
            {
                "System.Data.SqlClient",
                "MySql.Data.MySqlClient"
            };

            this.model = new MigrationModel
            {
                ProviderName = this.ProviderNames[0],
                ValidationMode = ValidationMode.Submit
            };

            this.InitAssemblyCommand();

            this.InitUpAllCommand();
            this.InitDownAllCommand();
            this.InitUpOneCommand();
            this.InitDownOneCommand();

            this.InitSeedAllCommand();
            this.InitSeedOneCommand();

            this.InitGenMigrationScriptCommand();
            this.InitGenSeedScriptCommand();  
        }

        public void RestoreConnectionStrings()
        {
           this.StoredConnectionStrings = Session.Restore();
        }

        public void SaveConnectionString(string connectionString)
        {
            Session.AddConnectionString(connectionString);
            Session.Save();
        }

        public void SetActive()
        {
            this.IsActive = Visibility.Visible;
        }

        public void SetInactive()
        {
            this.IsActive = Visibility.Collapsed;
        }

        public void InitAssemblyCommand()
        {
            this.AssemblyPathCommand = new RelayCommand(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Dll, Exe (*.dll,*.exe)|*.dll;*.exe";
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        this.Model.AssemblyPath = dialog.FileName;

                        var migrationTypes = this.migrationService.ResolveMigrationTypes(this.model.AssemblyPath);
                        this.Migrations = new ObservableCollection<RecognizedMigrationFile>(migrationTypes);

                        var seederTypes = this.migrationService.ResolveSeederTypes(this.model.AssemblyPath);
                        this.Seeders = new ObservableCollection<RecognizedMigrationFile>(seederTypes);

                        this.RaisePropertyChanged("HasMigrations");
                        this.RaisePropertyChanged("HasSeeders");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error");
                    }

                }
            });
        }

        public void InitUpAllCommand()
        {
            this.UpAllCommand = new RelayCommand(async () =>
            {
                this.model.ValidateAll();
                if (!this.model.HasErrors)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            this.SetActive();

                            this.migrationService.UpAll(this.model.AssemblyPath, this.model.ConnectionString, this.model.ProviderName, this.model.Engine);

                            this.SetInactive();

                            MessageBox.Show("Db udpated!", "Success");

                            this.SaveConnectionString(this.model.ConnectionString);
                        }
                        catch (Exception e)
                        {
                            this.SetInactive();
                            MessageBox.Show(e.Message, "Error");
                            this.migrationService.Clear();
                        }
                    });
                }
            });
        }

        public void InitDownAllCommand()
        {
            this.DownAllCommand = new RelayCommand(async () =>
            {
                this.model.ValidateAll();
                if (!this.model.HasErrors)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            this.SetActive();

                            this.migrationService.DownAll(this.model.AssemblyPath, this.model.ConnectionString, this.model.ProviderName, this.model.Engine);

                            this.SetInactive();

                            MessageBox.Show("Db udpated!", "Success");

                            this.SaveConnectionString(this.model.ConnectionString);
                        }
                        catch (Exception e)
                        {
                            this.SetInactive();
                            MessageBox.Show(e.Message, "Error");
                            this.migrationService.Clear();
                        }
                    });
                }
            });
        }

        public void InitUpOneCommand()
        {
            this.UpOneCommand = new RelayCommand<RecognizedMigrationFile>(async (file) =>
            {
                if (file != null)
                {
                    this.model.ValidateAll();
                    if (!this.model.HasErrors)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                this.SetActive();

                                this.migrationService.UpOne(file.FullName, this.model.AssemblyPath, this.model.ConnectionString, this.model.ProviderName, this.model.Engine);

                                this.SetInactive();

                                MessageBox.Show("Db udpated!", "Success");

                                this.SaveConnectionString(this.model.ConnectionString);
                            }
                            catch (Exception e)
                            {
                                this.SetInactive();
                                MessageBox.Show(e.Message, "Error");
                                this.migrationService.Clear();
                            }
                        });

                    }
                }
            });
        }

        public void InitDownOneCommand()
        {
            this.DownOneCommand = new RelayCommand<RecognizedMigrationFile>(async (file) =>
            {
                if (file != null)
                {
                    this.model.ValidateAll();
                    if (!this.model.HasErrors)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                this.SetActive();

                                this.migrationService.DownOne(file.FullName, this.model.AssemblyPath, this.model.ConnectionString, this.model.ProviderName, this.model.Engine);

                                this.SetInactive();

                                MessageBox.Show("Db udpated!", "Success");

                                this.SaveConnectionString(this.model.ConnectionString);
                            }
                            catch (Exception e)
                            {
                                this.SetInactive();
                                MessageBox.Show(e.Message, "Error");
                                this.migrationService.Clear();
                            }
                        });
                    }
                }
            });
        }

        public void InitSeedAllCommand()
        {
            this.SeedAllCommand = new RelayCommand(async () =>
            {
                this.model.ValidateAll();
                if (!this.model.HasErrors)
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            this.SetActive();

                            this.migrationService.SeedAll(this.model.AssemblyPath, this.model.ConnectionString, this.model.ProviderName, this.model.Engine);

                            this.SetInactive();

                            MessageBox.Show("Db udpated!", "Success");

                            this.SaveConnectionString(this.model.ConnectionString);
                        }
                        catch (Exception e)
                        {
                            this.SetInactive();
                            MessageBox.Show(e.Message, "Error");
                            this.migrationService.Clear();
                        }
                    });
                }
            });
        }

        public void InitSeedOneCommand()
        {
            this.SeedOneCommand = new RelayCommand<RecognizedMigrationFile>(async (file) =>
            {
                if (file != null)
                {
                    this.model.ValidateAll();
                    if (!this.model.HasErrors)
                    {
                        await Task.Run(() =>
                        {
                            try
                            {
                                this.SetActive();

                                this.migrationService.SeedOne(file.FullName, this.model.AssemblyPath, this.model.ConnectionString, this.model.ProviderName, this.model.Engine);

                                this.SetInactive();

                                MessageBox.Show("Db udpated!", "Success");

                                this.SaveConnectionString(this.model.ConnectionString);
                            }
                            catch (Exception e)
                            {
                                this.SetInactive();
                                MessageBox.Show(e.Message, "Error");
                                this.migrationService.Clear();
                            }
                        });
                    }
                }
            });
        }

        public void InitGenMigrationScriptCommand()
        {
            this.GenMigrationScriptCommand = new RelayCommand(() =>
            {
                this.model.ClearErrors();
                this.model.ValidateProperty("AssemblyPath");
                if (!this.model.HasErrors)
                {
                    try
                    {
                        var directory = this.migrationService.GetAssemblyDirectory(this.model.AssemblyPath);

                        this.migrationService.CreateMigrationScript(this.model.AssemblyPath, this.model.ProviderName, directory + "/script.sql", directory + "/procedures.sql", this.model.Engine);

                        MessageBox.Show("Script created!", "Success");

                        Process.Start(directory);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error");
                        this.migrationService.Clear();
                    }
                }
            });
        }

        public void InitGenSeedScriptCommand()
        {
            this.GenSeedScriptCommand = new RelayCommand(() =>
            {
                this.model.ClearErrors();
                this.model.ValidateProperty("AssemblyPath");
                if (!this.model.HasErrors)
                {
                    try
                    {
                        var directory = this.migrationService.GetAssemblyDirectory(this.model.AssemblyPath);

                        this.migrationService.CreateSeedScript(this.model.AssemblyPath, this.model.ProviderName, directory + "/seed.sql");

                        MessageBox.Show("Script created!", "Success");

                        Process.Start(directory);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "Error");
                        this.migrationService.Clear();
                    }
                }
            });
        }

    }
}
