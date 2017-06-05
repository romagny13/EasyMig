# EasyMig

> Migration Tool and Services

Support:
* **Sql Server** 
* **MySQL**

Commands:
* **Drop Database**
* **Create Database**
* **Create Table**
    * _Add primary key_
    * _Add column_ (type, nullable, default, unique)
    * _Add foreign key_
    * _add timestamps_
    * _Insert data_
* **Alter Table**
    * _Add column_
    * _Modify column_
    * _Drop column_
    * _Add primary key constraint_
    * _Add foreign key constraint_
* **Drop Table**

* **SeedTable**
    * Insert dictionary of key/value

Execution:
* Do **Migrations All**: update database from Assembly path or Types or in Memory
* Do **Migration One** (only one migration file/Type)
* Do **Seed All**: from Assembly path or Types or in Memory
* Do **Seed One** (only one seeder)
* Get Migrations | Seeders **Query string**
* Create **Migration Script** (create table, etc.)
* Create **Seed Script**
* **Execute** a sql **Query**

Database information:
* Check if **Database exists**
* Check if **Table exists**
* Check if **Column exists**
* **Get Table** Schema with columns, primary key and foreign key definitions
* **Get** Table **rows**

**Version**:
Migrations and Seeders are **grouped** by name **and sorted** by version and name. Example:
"_001_CREATE_POSTS_TABLE" is before "_002_CREATE_POSTS_TABLE". Only the last is executed ("_002_CREATE_POSTS_TABLE" here)

"_001_CREATE_POSTS_TABLE" : the version is "_001_", the name is "CREATE_POSTS_TABLE", the full name is "_001_CREATE_POSTS_TABLE"


## Migrations and Seeders

Create a **Migration file**

```cs
public class CREATE_POSTS_TABLE : Migration
{
    public override void Up()
    {
        EasyMig.CreateTable("posts")
              .AddPrimaryKey("id") // key auto incremented
              .AddColumn("title")
              .AddColumn("content", ColumnType.Text())
              .AddTimestamps() // add created_at and updated_at columns
              .AddForeignKey("user_id", "users", "id")
              // we could initliaze table with data (identity off)
              .Insert(SeedData.New.Set("id", 1).Set("title", "Post 1").Set("content", "Content 1").Set("user_id", 1))
              .Insert(SeedData.New.Set("id", 2).Set("title", "Post 2").Set("content", "Content 2").Set("user_id", 1))
              .Insert(SeedData.New.Set("id", 3).Set("title", "Post 3").Set("content", "Content 3").Set("user_id", 2));

    }

    public override void Down()
    {
        EasyMig.DropTable("posts");
    }

}
```

Create a **Seeder file** 
Allow to seed an existing table

```cs
public class Posts_Seeder : Seeder
{
    public override void Run()
    {
        EasyMig.SeedTable("posts")
            // dictionary (string, object) or use SeeData helper
            .Insert(SeedData.New.Set("title", "Post 4").Set("content", "Content 4").Set("user_id", 3))
            .Insert(SeedData.New.Set("title", "Post 5").Set("content", "Content 5").Set("user_id", 3))
    }
}
```

Then use the EasyMig Tool is the easy way to select types and do migrations.

<img src="http://res.cloudinary.com/romagny13/image/upload/v1496624500/easymigapp_tool_sc0yol.png">

## In Memory

But its not a requirement. We could define migrations and seeders where we want and execute from Memory.

Example:
```cs
// define commands
EasyMig
    .AlterTable("posts")
    .AddForeignKeyConstraint("category_id", "categories", "id");

    // other commands ...

// execute
EasyMig.DoMigrationsFromMemory(@"Server=localhost\SQLEXPRESS;Database=db1;Trusted_Connection=True;", "System.Data.SqlClient");
```

## MySQL Engine

* **Default** is "**InnoDB**" (databases are created with relations and schema). But its possible to change to "**MyISAM**"

<img src="http://res.cloudinary.com/romagny13/image/upload/v1496622672/mysql_schema_bevqqq.png">

Change the engine. Example:
```cs
 EasyMig.DoMigrationsFromMemory("server=localhost;database=dbtest;uid=root", "MySql.Data.MySqlClient", "MyISAM");
 ```

## Fake Data

Example with [Faker.Net](https://github.com/slashdotdash/faker-cs)

```
PM> Install-Package Faker.Net
```

```cs
var table = EasyMig.CreateTable("users")
               .AddPrimaryKey("id")
               .AddColumn("firstname")
               .AddColumn("lastname")
               .AddColumn("age", ColumnType.Int())
               .AddColumn("email")
               .AddColumn("phone")
               .AddColumn("address")
               .AddColumn("zip")
               .AddColumn("city")
               .AddColumn("note", ColumnType.Text());

for (int i = 1; i< 100; i++)
{
    table.Insert(SeedData.New
    .Set("id", i)
    .Set("firstName", Faker.Name.First())
    .Set("lastname", Faker.Name.Last())
    .Set("age", Faker.RandomNumber.Next(20, 50))
    .Set("email", Faker.Internet.Email())
    .Set("phone", Faker.Phone.Number())
    .Set("address", Faker.Address.StreetAddress())
    .Set("zip", Faker.Address.ZipCode())
    .Set("city", Faker.Address.City())
    .Set("note", Faker.Lorem.Paragraph())
    );
}

//  var query = EasyMig.GetMigrationQuery("MySql.Data.MySqlClient");
// or
EasyMig.DoMigrationsFromMemory("server=localhost;database=dbtest;uid=root", "MySql.Data.MySqlClient");

// or with Sql Server
// EasyMig.DoMigrationsFromMemory(@"Server=localhost\SQLEXPRESS;Database=db1;Trusted_Connection=True;", "System.Data.SqlClient");
```