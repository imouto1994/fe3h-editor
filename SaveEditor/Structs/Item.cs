using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Item
    {
        public const int SIZE = 0x4;

        public short Id;
        public byte Durability;
        public byte Amount;

        public bool IsUnlimitedDurability => Durability == 100;

        public void Init()
        {
            Id = -1;
            Durability = 0;
            Amount = 0;
        }

        public string EquippedName => $"{Database.GetItemName(Id)}, {Durability}/{Database.GetItemDurability(Id)}";

        public override string ToString()
        {
            if (Durability == 100)
            {
                return $"{Database.GetItemName(Id)}, ∞, x{Amount}";
            }

            return $"{Database.GetItemName(Id)}, {Durability}/{Database.GetItemDurability(Id)}, x{Amount}";
        }
    }
    
    public class ItemCompare : IComparer<Item>
    {
        public int Compare(Item i1, Item i2)
        {
            int result;  

            if (i1.Id == -1)  
            {  
                result = 1;  
            }  
            else if (i2.Id == -1)  
            {  
                result = -1;  
            }  
            else
            {
                if (i1.Id > i2.Id)
                    result = 1;
                else if (i1.Id < i2.Id)
                    result = -1;
                else
                {
                    result = Util.NumberCompare(i1.Durability, i2.Durability);
                    if (result == 0)
                    {
                        result = Util.NumberCompare(i1.Durability, i2.Durability);
                    }
                }
            }  

            return result; 
        }
    }

}
