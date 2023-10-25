using Npgsql;

namespace Library;

public record Lot(int Id, string Address, int Capacity)
{
  public async Task Add(NpgsqlConnection connection)
  {
    string commandText = $"INSERT INTO lots (address, capacity) VALUES (@address, @capacity)";
    await using var cmd = new NpgsqlCommand(commandText, connection);
    cmd.Parameters.AddWithValue("address", Address);
    cmd.Parameters.AddWithValue("capacity", Capacity);
    await cmd.ExecuteNonQueryAsync();
  }
}
