using Npgsql;

namespace Library;

public record Slot(int Id, Lot Lot, DateTime Entry, DateTime Exit, float Price, float Discount)
{
  public async Task<int?> Add(NpgsqlConnection connection)
  {
    string commandText = $"INSERT INTO slots (lot_id, entry_date, exit_date, price, discount_percetage) VALUES (@lot_id, @entry_date, @exit_date, @price, @discount_percetage)";
    await using var cmd = new NpgsqlCommand(commandText, connection);
    cmd.Parameters.AddWithValue("lot_id", Lot.Id);
    cmd.Parameters.AddWithValue("entry_date", Entry);
    cmd.Parameters.AddWithValue("exit_date", Exit);
    cmd.Parameters.AddWithValue("discount_percetage", Discount);
    return (int?)await cmd.ExecuteScalarAsync();
  }
}
