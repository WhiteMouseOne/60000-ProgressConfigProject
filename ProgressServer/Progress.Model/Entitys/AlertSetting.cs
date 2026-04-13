using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    public class AlertSetting
    {
        [Key] public int Id { get; set; }
        public int LeadDays { get; set; } = 3;
        public bool Enabled { get; set; } = true;
    }
}
