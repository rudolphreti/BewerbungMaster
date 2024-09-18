using BewerbungMasterApp.Models;

namespace BewerbungMasterApp.Services
{
    public class JobEditService
    {
        private (int Index, string Field) ActiveEditField { get; set; } = (-1, string.Empty);

        public void SetActiveEditField(int index, string field)
        {
            ActiveEditField = (index, field);
        }

        public void ClearActiveEditField()
        {
            ActiveEditField = (-1, string.Empty);
        }

        public bool IsEditingPosition(int index)
        {
            return ActiveEditField.Index == index && ActiveEditField.Field == "position";
        }

        public bool IsEditingCompany(int index)
        {
            return ActiveEditField.Index == index && ActiveEditField.Field == "company";
        }
    }
}