using Npgsql;

namespace Library;

public record Customer(int Id, string Name)
{
  public async Task<Slot> Enter(Lot lot, DateTime exit, NpgsqlConnection connection)
  {
    var slot = new Slot(0, lot, DateTime.Now, exit, 10, 1);
    int slotId = (int)await slot.Add(connection);
    string commandText = $"INSERT INTO customers VALUES @name, @debt";
    await using var cmd = new NpgsqlCommand(commandText, connection);
    cmd.Parameters.AddWithValue("slot_id", slotId);
    cmd.Parameters.AddWithValue("name", Name);
    await cmd.ExecuteNonQueryAsync();
    return new Slot(slotId, slot.Lot, slot.Entry, slot.Exit, slot.Price, slot.Discount);
  }

  public async Task<float> Leave(Slot slot, NpgsqlConnection connection)
  {
    var time = slot.Exit - slot.Entry;
    var cost = time.Hours * slot.Price;
    if (DateTime.Now > slot.Exit)
    {
      var dept = DateTime.Now - slot.Exit;
      cost += slot.Price * dept.Hours;
    } else {
      cost -= cost * slot.Discount / 100;
    }
    return cost;
  }
}
