﻿namespace CallaApp.Models
{
    public class Banner: BaseEntity
    {
        public string Image { get; set; }
        public bool IsLarge { get; set; } = false;
    }
}
