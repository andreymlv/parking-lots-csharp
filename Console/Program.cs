using Npgsql;
using Library;

var connString = "Host=localhost;Username=super_user;Password=pass;Database=super_db";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

string createTableQuery = @"
DROP TABLE IF EXISTS lots;
DROP TABLE IF EXISTS slots;
DROP TABLE IF EXISTS clients;
DROP TABLE IF EXISTS cars;
DROP TABLE IF EXISTS entries;

CREATE TABLE lots (
  id SERIAL PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  capacity INT NOT NULL,
);

CREATE TABLE slots (
    id SERIAL PRIMARY KEY,
    lot serial NOT NULL REFERENCES lots (id),
    number INT NOT NULL,
    price DECIMAL NOT NULL
);

CREATE TABLE clients (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    discount DECIMAL NOT NULL,
    debt DECIMAL NOT NULL
);

CREATE TABLE cars (
    id SERIAL PRIMARY KEY,
    client INT NOT NULL,
    number VARCHAR(255) NOT NULL,
    brand VARCHAR(255) NOT NULL,
    FOREIGN KEY (client) REFERENCES clients (id)
);

CREATE TABLE entries (
    id SERIAL PRIMARY KEY,
    slot INT NOT NULL,
    car INT NOT NULL,
    entry TIMESTAMP NOT NULL,
    exit TIMESTAMP NOT NULL,
    FOREIGN KEY (slot) REFERENCES slots (id),
    FOREIGN KEY (car) REFERENCES cars (id)
);
";
await using (var cmd = new NpgsqlCommand(createTableQuery, conn))
{
  await cmd.ExecuteNonQueryAsync();
}

// Add sample data to db
var lots = new List<Lot>
{
    new(1, "Tver", 10),
    new(2, "Moscow", 3),
    new(3, "Leningrad", 0)
};
foreach (var lot in lots)
{
  await lot.Add(conn);
}
var andrey = new Customer(1, "Andrey Malov");
var slot = await andrey.Enter(lots[0], new DateTime(2023, 10, 17), conn);
var price = await andrey.Leave(slot, conn);

Console.WriteLine($"Price {price}");

