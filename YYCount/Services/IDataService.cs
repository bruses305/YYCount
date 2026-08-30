using System.Windows.Documents;
using YYCount.Models;

namespace YYCount.Services;

public interface IDataService
{
    List<EquipmentItem> LoadEquipment();
    void SaveEquipment(List<EquipmentItem> items);
    List<Calculation> LoadCalculations();
    void SaveCalculations(List<Calculation> calculations);
}