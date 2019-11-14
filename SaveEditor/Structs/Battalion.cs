using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Battalion
    {
        public const int SIZE = 0x8;

        public short CharacterId;
        public ushort Exp, Stamina;
        public byte Type, Skill;

        public void Init()
        {
            CharacterId = -1;
            Exp = 0;
            Stamina = 0;
            Type = Database.BATTALION_COUNT;
            Skill = Database.COMBAT_ARTS_COUNT;
        }

        public override string ToString()
        {
            return $"{Database.GetBattalionName(Type)}";
        }
        
        public string GetBarracksName()
        {
            return $"{Database.GetBattalionName(Type)}  ({(CharacterId == -1 ? Database.STR_NONE : Database.GetUnitName(CharacterId))})";
        }

        public string GenerateDebugOut()
        {
            return $"Character Id:{CharacterId}, Type:{Database.GetBattalionName(Type, true)}, Exp:{Exp}, Stamina:{Stamina}, Unk1:{Skill}";
        }
    }


    public class BattalionCompare : IComparer<Battalion>
    {
        public int Compare(Battalion i1, Battalion i2)
        {
            int result;  

            if (i1.Type == Database.BATTALION_COUNT)  
            {  
                result = 1;  
            }  
            else if (i2.Type == Database.BATTALION_COUNT)  
            {  
                result = -1;  
            }  
            else
            {
                if (i1.Type > i2.Type)
                    result = 1;
                else if (i1.Type < i2.Type)
                    result = -1;
                else
                {
                    result = Util.NumberCompare(i1.CharacterId, i2.CharacterId);
                    if (result == 0)
                    {
                        result = Util.NumberCompare(i1.CharacterId, i2.CharacterId);
                    }
                }
            }  

            return result; 
        }
    }

}
