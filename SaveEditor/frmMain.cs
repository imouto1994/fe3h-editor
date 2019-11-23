using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SaveEditor.Structs;

namespace SaveEditor
{
    public partial class frmMain : Form
    {
        private Save save;
        private enmLanguage currentLanguage;
		private string curFile;
		private bool IsLoading, IsUpdating;

        private const bool IS_DEBUG_MODE = false;

        public frmMain()
        {
            InitializeComponent();

            numPlaytime.Maximum = Database.MAX_PLAYTIME;
            numMoney.Maximum = Database.MAX_MONEY;
            numReputation.Maximum = Database.MAX_REPUTATION;
            numInstructExp.Maximum = Database.MAX_INSTRUCT_EXP;

            numCurrentClassLevel.Maximum = Database.MAX_CLASS_LEVEL;
            numSetClassLevel.Maximum = Database.MAX_CLASS_LEVEL;
            numRNG.Maximum = uint.MaxValue;


            lblSupportTalkNote.Text = "Note:\r\n";
            lblSupportTalkNote.Text += "None = 0   - 100,\r\n";
            lblSupportTalkNote.Text += "   C = 101 - 300,\r\n";
            lblSupportTalkNote.Text += "   B = 301 - 600,\r\n";
            lblSupportTalkNote.Text += "   A = 601 - 900,\r\n";
            lblSupportTalkNote.Text += "   S = 901 - 1200\r\n";
            lblSupportTalkNote.Text += "if \">>\" exists, then add +200 to all values after them";
        }

		#region Form Events

        private void frmMain_Load(object sender, EventArgs e)
        {
            Database.Init();
            CleanUp();
			
            if(IS_DEBUG_MODE) MemoryHelper.GetMemoryAddresses();

			cboLanguage.Items.Clear();
			
            foreach (enmLanguage enm in Enum.GetValues(typeof(enmLanguage)))
            {
                cboLanguage.Items.Add(enm.GetDescription());
            }

            cboLanguage.SelectedIndex = (int)enmLanguage.en_u;
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            curFile = files[0];
            LoadSave(curFile);
        }

        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeLanguage();
        }
     
        private void mnuLoadSave_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.Title = @"Load Save File";
                ofd.Filter = @"Fire Emblem Save Files|auto;slot*|All Files|*.*";
                ofd.InitialDirectory = Application.StartupPath;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    curFile = ofd.FileName;
                    LoadSave(curFile);
                }
            }
        }

        private void mnuWriteSave_Click(object sender, EventArgs e)
        {
            if (save == null)
            {
                MessageBox.Show(@"You can't write a save, before loading it.", @"No save loaded!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = @"Write Save File";
                sfd.Filter = @"Fire Emblem Save Files|auto;slot*|All Files|*.*";
                sfd.FileName = Path.GetFileName(curFile);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    WriteSave(sfd.FileName);
                }
            }
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void btnGetMiscItems_Click(object sender, EventArgs e)
        {
            if(save == null)
                return;

            for (int i = 0; i < save.SaveData.Player.MiscItems.Length; i++)
                save.SaveData.Player.MiscItems[i] = 99;

            LoadItems2();
        }

        private void btnGetGiftItems_Click(object sender, EventArgs e)
        {
            if(save == null)
                return;

            for (int i = 0; i < save.SaveData.Player.GiftItems.Length; i++)
                save.SaveData.Player.GiftItems[i] = 99;

            LoadItems2();
        }

        private void btnSortBattalion_Click(object sender, EventArgs e)
        {
            if(save == null)
                return;

            var comparer = new BattalionCompare();
            Array.Sort(save.SaveData.Player.Battalions, comparer.Compare);

            LoadBattalions();
        }

        private void btnSaveItem_Click(object sender, EventArgs e)
        {
            SaveItem();
        }
      
        private void btnSortItems_Click(object sender, EventArgs e)
        {
            if(save == null)
                return;

            var comparer = new ItemCompare();
            Array.Sort(save.SaveData.Items, comparer.Compare);

            LoadItems();
        }
                
        private void btnAddEssentialItems_Click(object sender, EventArgs e)
        {	
            if(save == null)
                return;

            foreach (var id in Database.EssentialItems)
            {
                AddItem(id);
            }

            var comparer = new ItemCompare();
            Array.Sort(save.SaveData.Items, comparer.Compare);

            UpdateItemCount();
            LoadItems();
        }
              
        private void btnSetDurability_Click(object sender, EventArgs e)
        {
            if(save == null)
                return;

            for (int i = 0; i < save.SaveData.Items.Length; i++)
            {
                var item = save.SaveData.Items[i];

                if (item.Id == -1) continue;

                if (rdoItemsMax.Checked)
                {
                    item.Durability = (byte)Database.GetItemDurability(item.Id);
                }
                else if (rdoItems100.Checked)
                {
                    item.Durability = 100;
                }
                else if (rdoMaxWeapon.Checked)
                {
                    if (item.Id >= 10 && item.Id < 510)
                    {
                        item.Durability = 100;
                    }
                    else
                    {
                        item.Durability = (byte)Database.GetItemDurability(item.Id);
                    }
                }

                save.SaveData.Items[i] = item;
            }
            
            LoadItems();
        }

        private void btnSaveChara_Click(object sender, EventArgs e)
        {
            SaveCharacter();
        }
  
        private void btnSaveBattalion_Click(object sender, EventArgs e)
        {
            SaveBattalion();
        }
        
        private void btnExportChara_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;
			
            if(x == -1 || save == null)
                return;

            byte[] data = Util.StructureToByteArray(save.SaveData.Characters[x]);

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = @"Save Character File";
                sfd.Filter = @"Fire Emblem Character Files|*.character|All Files|*.*";
                sfd.InitialDirectory = Application.StartupPath;
                sfd.FileName = $@"{Database.GetUnitName(save.SaveData.Characters[x].data.Id, true)}.character";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                }
            }
        }

        private void btnImportChara_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;
			
            if(x == -1 || save == null)
                return;
            
            using (var ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.Title = @"Load Character File";
                ofd.Filter = @"Fire Emblem Character Files|*.character|All Files|*.*";
                ofd.InitialDirectory = Application.StartupPath;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] data = File.ReadAllBytes(ofd.FileName);

                    if (data.Length == Character.SIZE)
                    {
                        save.SaveData.Characters[x] = Util.ReadStructure<Character>(data);
                    }
                    else
                    {
                        MessageBox.Show(@"Character file has an invalid filesize!", @"Invalid Filesize", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    LoadCharacters(x);
                }
            }
        }
        
        private void btnSetClassValues_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(x == -1 || lsvClassExp.SelectedItems.Count == 0 || save == null)
                return;

            var job = lsvClassExp.SelectedItems[0];

            int class_id = int.Parse(job.Tag.ToString());
            ushort exp = (ushort)Math.Min((int)numSetClassExp.Value, Database.GetMaxClassExp(class_id));
            byte level = (byte) numSetClassLevel.Value;

            save.SaveData.Characters[x].data.ClassExp[class_id] = exp;
            save.SaveData.Characters[x].data.ClassLevel[class_id] = level;

            if (save.SaveData.Characters[x].Class == class_id)
            {
                save.SaveData.Characters[x].data.CurrentClassExp = exp;
                save.SaveData.Characters[x].data.CurrentClassLevel = level;
            }
            
            LoadCharacters(x);
        }
        
        private void btnEditMiscItem_Click(object sender, EventArgs e)
        {
            if(lsvMiscItems.SelectedItems.Count == 0 || save == null)
                return;

            for (int i = 0; i < lsvMiscItems.SelectedItems.Count; i++)
            {
                var item = lsvMiscItems.SelectedItems[i];
                int id = int.Parse(item.Tag.ToString());
                save.SaveData.Player.MiscItems[id] = (byte)numEditMiscItem.Value;
            }

            LoadItems2();
        }

        private void btnEditGiftItem_Click(object sender, EventArgs e)
        {
            if(lsvGiftItems.SelectedItems.Count == 0 || save == null)
                return;
            
            for (int i = 0; i < lsvGiftItems.SelectedItems.Count; i++)
            {
                var item = lsvGiftItems.SelectedItems[i];
                int id = int.Parse(item.Tag.ToString());
                save.SaveData.Player.GiftItems[id] = (byte)numEditGiftItem.Value;
            }

            LoadItems2();
        }
        
        private void btnMaxSkillExp_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(x == -1 || save == null)
                return;
               
            IsUpdating = true;
            
            var chara1 = save.SaveData.Characters[x].data;

            numCharaSword.Value = GetMaxSkillExp(chara1.SkillLevel[0]);
            numCharaLance.Value = GetMaxSkillExp(chara1.SkillLevel[1]);
            numCharaAxe.Value = GetMaxSkillExp(chara1.SkillLevel[2]);
            numCharaBow.Value = GetMaxSkillExp(chara1.SkillLevel[3]);
            numCharaFighting.Value = GetMaxSkillExp(chara1.SkillLevel[4]);
            numCharaReason.Value = GetMaxSkillExp(chara1.SkillLevel[5]);
            numCharaFaith.Value = GetMaxSkillExp(chara1.SkillLevel[6]);
            numCharaAuthority.Value = GetMaxSkillExp(chara1.SkillLevel[7]);
            numCharaArmor.Value = GetMaxSkillExp(chara1.SkillLevel[8]);
            numCharaRiding.Value = GetMaxSkillExp(chara1.SkillLevel[9]);
            numCharaFly.Value = GetMaxSkillExp(chara1.SkillLevel[10]);

            IsUpdating = false;
        }
        
        private void btnUnlockAllPerks_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;
			
            if(x == -1 || save == null)
                return;
            
            for (int i = 0; i < chkAbilities.Items.Count; i++) chkAbilities.SetItemChecked(i, true);
        }

        private void btnUnlockAllCombatArts_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;
			
            if(x == -1 || save == null)
                return;

            for (int i = 0; i < chkCombatArts.Items.Count; i++) chkCombatArts.SetItemChecked(i, true);

        }
        
        private void btnQuestSetState_Click(object sender, EventArgs e)
        {
            if(lsvQuest.SelectedItems.Count == 0 || save == null)
                return;
                     
            for (int i = 0; i < lsvQuest.SelectedItems.Count; i++)
            {
                var item = lsvQuest.SelectedItems[i];
                int id = int.Parse(item.Tag.ToString());
                save.SaveData.Activities.QuestStateList[id] = (byte)cboQuestState.SelectedIndex;
            }

            var first_item = lsvQuest.SelectedItems[0];
            int first_item_id = int.Parse(first_item.Tag.ToString());

            LoadQuests(first_item_id);
        }
        
        private void btnMaxClassExp_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(x == -1 || save == null)
                return;

            for (int i = 0; i < Database.MAX_CLASS; i++)
            {
                save.SaveData.Characters[x].data.ClassExp[i] = (ushort)Database.GetMaxClassExp(i);
            }
            save.SaveData.Characters[x].data.CurrentClassExp = (ushort)Database.GetMaxClassExp(save.SaveData.Characters[x].Class);

            numCurrentClassExp.Value = save.SaveData.Characters[x].data.CurrentClassExp;

            LoadCharacterClassExp();
        }
        
        private void btnTalkSetState_Click(object sender, EventArgs e)
        {
            if(lsvTalk.SelectedItems.Count == 0 || save == null)
                return;
                   
            for (int i = 0; i < lsvTalk.SelectedItems.Count; i++)
            {
                var item = lsvTalk.SelectedItems[i];
                int id = int.Parse(item.Tag.ToString());
                save.SaveData.Player.CharacterSupportValues[id] = (ushort)numTalkSetState.Value;
            }

            var first_item = lsvTalk.SelectedItems[0];
            int first_item_id = int.Parse(first_item.Tag.ToString());

            LoadSupportTalk(first_item_id);
        }
        
        private void btnItemDurability_Click(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;
			
            if(x == -1 || save == null)
                return;

            var chara = save.SaveData.Characters[x];

            for (int i = 0; i < chara.data.ItemCount; i++)
            {
                var item = chara.data.Items[i];

                if (item.Id != -1)
                {
                    item.Durability = 100;
                }

                chara.data.Items[i] = item;
            }

            save.SaveData.Characters[x] = chara;

            LoadCharacters(x);
        }
  
		#endregion

		#region data processing

        private void FillForm()
        {
            try
            {
                LoadPlayer();
                LoadSupportTalk();
                LoadActivities();
                LoadQuests();
                LoadItems();
                LoadItems2();
                LoadCharacters();
                LoadBattalions();
                LoadDatabaseViewer();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(e.ToString());
            }
        }

        private void CleanUp()
        {
            lstItems.Items.Clear();
            lstCharacter.Items.Clear();
            lstCharacterItems.Items.Clear();
            lstBattalion.Items.Clear();
            lsvClassExp.Items.Clear();
            lsvGiftItems.Items.Clear();
            lsvMiscItems.Items.Clear();
            
            save = null;
        }

        private void InitComboAndChecklists()
        {
            Util.SetComboBoxDataSource(cboItemEditor, Database.ItemList);
            Util.SetComboBoxDataSource(cboBattalionType, Database.BattalionList);

            Util.SetComboBoxDataSource(cboCharaId, Database.UnitList);
            Util.SetComboBoxDataSource(cboCharaBattalion, Database.BattalionList);
            Util.SetComboBoxDataSource(cboAdjutant, Database.CharacterList);

            Util.SetComboBoxDataSource(cboAbility1, Database.AbilityList);
            Util.SetComboBoxDataSource(cboAbility2, Database.AbilityList);
            Util.SetComboBoxDataSource(cboAbility3, Database.AbilityList);
            Util.SetComboBoxDataSource(cboAbility4, Database.AbilityList);
            Util.SetComboBoxDataSource(cboAbility5, Database.AbilityList);

            Util.SetComboBoxDataSource(cboBattleSkill1, Database.CombatArtList);
            Util.SetComboBoxDataSource(cboBattleSkill2, Database.CombatArtList);
            Util.SetComboBoxDataSource(cboBattleSkill3, Database.CombatArtList);

            Util.SetComboBoxDataSource(cboBattalionCharacter, Database.CharacterList);

            Util.SetComboBoxDataSource(cboBattalionSkill, Database.BattalionSkillList);

            Util.SetComboBoxDataSource(cboClass, Database.ClassList);

            cboDifficulty.Items.Clear();
            for (int i = 0; i < Database.DIFFICULTY_COUNT; i++) cboDifficulty.Items.Add(Database.GetString(373 + i, 1));

            cboGamestyle.Items.Clear();
            for (int i = 0; i < Database.GAMESTYLE_COUNT; i++) cboGamestyle.Items.Add(Database.GetString(381 + i, 1));

            chkCharaClassUnlockFlags.Items.Clear();
            for (int i = 0; i < Database.CLASS_FLAGS_COUNT; i++) chkCharaClassUnlockFlags.Items.Add(Database.GetClassName(i));

            chkAbilities.Items.Clear();
            for (int i = 0; i < Database.ABILITY_COUNT; i++) chkAbilities.Items.Add(Database.GetAbilityName(i));

            chkCombatArts.Items.Clear();
            for (int i = 0; i < Database.COMBAT_ARTS_COUNT; i++) chkCombatArts.Items.Add(Database.GetCombatArtName(i));
            
            chkCharaFlags.Items.Clear();
            foreach (enmCharacterFlags t in Enum.GetValues(typeof(enmCharacterFlags)))
            {
                if (t == enmCharacterFlags.None) continue;
                chkCharaFlags.Items.Add(t.GetDescription());
            }

            chkCharaClassFlags.Items.Clear();
            foreach (enmCharacterClassFlags t in Enum.GetValues(typeof(enmCharacterClassFlags)))
            {
                if (t == enmCharacterClassFlags.None) continue;
                chkCharaClassFlags.Items.Add(t.GetDescription());
            }

            cboQuestState.Items.Clear();
            foreach (enmQuestState t in Enum.GetValues(typeof(enmQuestState)))
                cboQuestState.Items.Add($@"{(int)t} - {t.GetDescription()}");
        }

        private void ChangeLanguage()
        {
            currentLanguage = (enmLanguage) cboLanguage.SelectedIndex;
            Database.Init(currentLanguage, false);
            InitComboAndChecklists();

            string STR_EXP = Database.GetString(2324, 1);
            string STR_LEVEL = Database.GetString(2326, 1);
            string STR_CLASS = Database.GetString(540, 1);
            string STR_ABILITIES = Database.GetString(260, 1);
            string STR_COMBAT_ARTS = Database.GetString(264, 1);
            string STR_ALL = Database.GetString(788, 1);

            //main tab
            grpPlayer.Text = $@"{Database.GetString(1455, 1)}";
            lblMoney.Text = $@"{Database.GetString(652, 1)}:";
            lblInstructLevel.Text = $@"{Database.GetString(1153, 1)}:";
            lblReputation.Text = $@"{Database.GetString(651, 1)}:";

            //grpGameSettings
            lblDifficulty.Text = $@"{Database.GetString(970, 1)}:";
            
            grpStatistics.Text = $@"{Database.GetString(3937, 1)} (current month only)";
            lblPlayLog_Wark.Text = $@"{Database.GetString(1025, 1)}:";
            lblPlayLog_Lecture.Text = $@"{Database.GetString(1026, 1)}:";
            lblPlayLog_ToBtl.Text = $@"{Database.GetString(1027, 1)}:";
            lblPlayLog_Rest.Text = $@"{Database.GetString(1212, 1)}:";
            lblPlayLog_Trnmnt.Text = $@"{Database.GetString(405, 1)}:";
            lblPlayLog_Sing.Text = $@"{Database.GetString(402, 1)}:";
            lblPlayLog_Lunch.Text = $@"{Database.GetString(401, 1)}:";
            lblPlayLog_Cooking.Text = $@"{Database.GetString(555, 1)}:";
            lblPlayLog_Drill.Text = $@"{Database.GetString(1860, 1)}:";
            lblPlayLog_Teaparty.Text = $@"{Database.GetString(3696, 1)}:";
            lblPlayLog_SCOUT.Text = $@"{Database.GetString(1820, 1)}:";

            grpActivityPoints.Text = $@"{Database.GetString(1163, 1)}:"; 
            lblActivityExplore.Text = $@"{Database.GetString(1025, 1)}:";
            lblActivityLesson.Text = $@"{Database.GetString(1026, 1)}:";
            lblActivityBattle.Text = $@"{Database.GetString(1027, 1)}:";

            grpGoddessStatue.Text = $@"{Database.GetString(461, 1)}:"; 
            lblStatue1.Text = $@"{Database.GetString(9760)}:";
            lblStatue2.Text = $@"{Database.GetString(9761)}:";
            lblStatue3.Text = $@"{Database.GetString(9762)}:";
            lblStatue4.Text = $@"{Database.GetString(9763)}:";

            //grpSettings.Text = $@"{Database.GetString(1019, 1)}:";
            
            //storage tabs
            tabStorage.Text = $@"{Database.GetString(1814, 1)}"; //1812
            grpItemList.Text = $@"{Database.GetString(1814, 1)} List";
            grpGiftItems.Text = $@"{Database.GetString(1853, 1)} / {Database.GetString(1854, 1)}";

            //character tab
            tabCharacter.Text = $@"{Database.GetString(1807, 1)}";
            grpCharacterList.Text = $@"{Database.GetString(1807, 1)} List";
            
            lblLevel.Text = $@"{STR_LEVEL}:";
            lblExp.Text = $@"{STR_EXP}:";
            lblMotivation.Text = @"Motivation:"; //1146, 1, no standalone string available
            lblAdjutant.Text = @"Adjutant:"; //3871, 1, no standalone string available
            lblCharaBattalion.Text = $@"{Database.GetString(670, 1)}:";
                
            tabStats.Text = $@"{Database.GetString(893, 1)}";
            grpStats.Text = $@"{Database.GetString(565, 1)}:";
            grpPassiveSkills.Text = $@"{STR_ABILITIES}:";
            grpBattleSkills.Text = $@"{STR_COMBAT_ARTS}:";
            lblHP.Text = $@"{Database.GetString(2325, 1)}:";
            lblStrength.Text = $@"{Database.GetString(2306, 1)}:";
            lblMagic.Text = $@"{Database.GetString(2307, 1)}:";
            lblDexterity.Text = $@"{Database.GetString(2308, 1)}:";
            lblSpeed.Text = $@"{Database.GetString(2309, 1)}:";
            lblLuck.Text = $@"{Database.GetString(2310, 1)}:";
            lblDefense.Text = $@"{Database.GetString(2311, 1)}:";
            lblResistance.Text = $@"{Database.GetString(2312, 1)}:";
            lblMovement.Text = $@"{Database.GetString(2313, 1)}:";
            lblCharm.Text = $@"{Database.GetString(2314, 1)}:";

            tabSkills.Text = $@"{Database.GetString(569, 1)}";
            grpSkillLevel.Text = $@"{Database.GetString(1367, 1)}:";
            grpLearnedMagic.Text = $@"{Database.GetString(1075, 1)}:";
            lblSword.Text = $@"{Database.GetString(7210)}:";
            lblLance.Text = $@"{Database.GetString(7211)}:";
            lblAxe.Text = $@"{Database.GetString(7212)}:";
            lblBow.Text = $@"{Database.GetString(7213)}:";
            lblFighting.Text = $@"{Database.GetString(7214)}:";
            lblReason.Text = $@"{Database.GetString(7215)}:";
            lblFaith.Text = $@"{Database.GetString(7216)}:";
            lblAuthority.Text = $@"{Database.GetString(7217)}:";
            lblArmor.Text = $@"{Database.GetString(7218)}:";
            lblRiding.Text = $@"{Database.GetString(7219)}:";
            lblFly.Text = $@"{Database.GetString(7220)}:";

            tabClassExp.Text = STR_CLASS;

            tabClassFlags.Text = $@"{STR_CLASS} (Flags)";
            clmnClassId.Text = STR_CLASS;
            clmnClassExp.Text = STR_EXP;
            clmnClassLevel.Text = STR_LEVEL;
            lblClassExp.Text = $@"{STR_EXP}:";
            lblClassLevel.Text = $@"{STR_LEVEL}:";
            lblCurrentClassExp.Text = $@"{STR_EXP}:";
            lblCurrentClassLevel.Text = $@"{STR_LEVEL}:";

            tabAbilities.Text = STR_ABILITIES;
            btnUnlockAllAbilities.Text = $@"{STR_ALL} {STR_ABILITIES}";
            tabCombatArts.Text = STR_COMBAT_ARTS;
            btnUnlockAllBattleSkills.Text = $@"{STR_ALL} {STR_COMBAT_ARTS}";

            //battalion tab
            tabBattalion.Text = $@"{Database.GetString(385, 1)}";
            grpBattalionList.Text = $@"{Database.GetString(670, 1)} List";
            grpBattalionEditor.Text = $@"{Database.GetString(670, 1)} Editor";

            //quest tab
            tabQuest.Text = $@"{Database.GetString(1811, 1)}";
            grpQuestList.Text = $@"{Database.GetString(1811, 1)} List";
            grpQuestEditor.Text = $@"{Database.GetString(1811, 1)} Editor";

            //talk tab
            tabTalk.Text = $@"{Database.GetString(1060, 1)}";
            grpTalkList.Text = $@"{Database.GetString(1060, 1)} List";
            grpTalkEditor.Text = $@"{Database.GetString(1060, 1)} Editor";

            FillForm();
        }

		private void LoadSave(string sPath)
		{
			IsLoading = true;
			
            try
            {
                long fSize = Util.GetFileSize(sPath);

                save = new Save();

                if (fSize == Save.SIZE_VERSION_1000)
                {
                    save.Read(sPath);
                }
                else if (fSize == Save.SIZE_VERSION_1001)
                {
                    save.Read(sPath);
                }
                else if (fSize >= 0x1D2000 && fSize < 0x200000)
                {
                    MemoryDump mem = new MemoryDump();
                    mem.Load(ref save, sPath);
                }
                else
                {
                    throw new NotSupportedException("This file is not supported, invalid filesize!");
                }

                if (IS_DEBUG_MODE)
                {
                    File.WriteAllBytes("Player.dat", Util.StructureToByteArray(save.SaveData.Player));
                    File.WriteAllBytes("Activities.dat", Util.StructureToByteArray(save.SaveData.Activities));
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (save.WarnChecksum)
            {
                MessageBox.Show($@"Calculated: 0x{save.CalculatedChecksum:X} != Read: 0x{save.Checksum:X}", @"Invalid Checksum", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ChangeLanguage(); //FillForm();
			
			IsLoading = false;
		}

        private void WriteSave(string sPath)
        {
            if(save == null)
                return;

            SavePlayer();
            SaveActivities();

            save.Write(sPath);
        }

        private void LoadPlayer()
        {
            if(save == null)
                return;

            Player player = save.SaveData.Player;

            numPlaytime.Value = Math.Min((int)player.Playtime, Database.MAX_PLAYTIME);
            numMoney.Value = Math.Min((int)player.Money, Database.MAX_MONEY);

            numChapter.Value = player.Chapter;
            cboDifficulty.SelectedIndex = Math.Min((int)player.Difficulty, Database.DIFFICULTY_COUNT);
            cboGamestyle.SelectedIndex = Math.Min((int)player.Gamestyle, Database.GAMESTYLE_COUNT);
            numRoute.Value = player.Route;
            numMapID.Value = player.MapID;
        }

        private void LoadSupportTalk(int index = -1)
        {            
            if(save == null)
                return;

            lsvTalk.Items.Clear();
            
            for (int i = 0; i < save.SaveData.Player.CharacterSupportValues.Length; i++)
            {
                var entry = new ListViewItem {Text = $@"{i:D3} - {Database.GetSupportTalkName(i)}", Tag = i.ToString()};
                entry.SubItems.Add(save.SaveData.Player.CharacterSupportValues[i].ToString());

                lsvTalk.Items.Add(entry);
            }

            SetListViewIndex(lsvTalk, index);
        }

        private void SavePlayer()
        {
            if(save == null)
                return;
            
            save.SaveData.Player.Playtime = (uint) numPlaytime.Value;
            save.SaveData.Player.Money = (uint) numMoney.Value;
            save.SaveData.Player.Chapter = (byte)numChapter.Value;
            save.SaveData.Player.Difficulty = (byte)cboDifficulty.SelectedIndex;
            save.SaveData.Player.Gamestyle = (byte)cboGamestyle.SelectedIndex;
            save.SaveData.Player.Route = (byte)numRoute.Value;
            save.SaveData.Player.MapID = (byte)numMapID.Value;
        }

        private void LoadActivities()
        {
            if(save == null)
                return;

            Activities activities = save.SaveData.Activities;

            numReputation.Value = Math.Min(activities.Reputation, Database.MAX_REPUTATION);
            numInstructExp.Value = Math.Min((int)activities.InstructExp, Database.MAX_INSTRUCT_EXP);
            
            lblInstructRank.Text = activities.GetInstructRank();
            numPlayLog_Wark.Value = activities.PlayLog_Wark;
            numPlayLog_Lecture.Value = activities.PlayLog_Lecture;
            numPlayLog_ToBtl.Value = activities.PlayLog_ToBtl;
            numPlayLog_Rest.Value = activities.PlayLog_Rest;
            numPlayLog_Trnmnt.Value = activities.PlayLog_Trnmnt;
            numPlayLog_Sing.Value = activities.PlayLog_Sing;
            numPlayLog_Lunch.Value = activities.PlayLog_Lunch;
            numPlayLog_Cooking.Value = activities.PlayLog_Cooking;
            numPlayLog_Drill.Value = activities.PlayLog_Drill;
            numPlayLog_Teaparty.Value = activities.PlayLog_Teaparty;
            numPlayLog_SCOUT.Value = activities.PlayLog_SCOUT;

            numActivityExplore.Value = activities.ActivityExplore;
            numActivityLesson.Value = activities.ActivityLesson;
            numActivityBattle.Value = activities.ActivityBattle;

            numStatue1.Value = activities.Statue1;
            numStatue2.Value = activities.Statue2;
            numStatue3.Value = activities.Statue3;
            numStatue4.Value = activities.Statue4;
        }

        private void LoadQuests(int index = -1)
        {
            if(save == null)
                return;
            
            lsvQuest.Items.Clear();
            
            for (int i = 0; i < save.SaveData.Activities.QuestStateList.Length; i++)
            {
                var entry = new ListViewItem {Text = $@"{i:D3} - {Database.GetQuestName(i)}", Tag = i.ToString()};
                entry.SubItems.Add(save.SaveData.Activities.QuestStateList[i].ToString());

                lsvQuest.Items.Add(entry);
            }

            SetListViewIndex(lsvQuest, index);
        }

        private void SaveActivities()
        {
            if(save == null)
                return;
            
            save.SaveData.Activities.InstructExp = (ushort)numInstructExp.Value;
            save.SaveData.Activities.Reputation = (uint)numReputation.Value;

            save.SaveData.Activities.PlayLog_Wark = (byte)numPlayLog_Wark.Value;
            save.SaveData.Activities.PlayLog_Lecture = (byte)numPlayLog_Lecture.Value;
            save.SaveData.Activities.PlayLog_ToBtl = (byte)numPlayLog_ToBtl.Value;
            save.SaveData.Activities.PlayLog_Rest = (byte)numPlayLog_Rest.Value;
            save.SaveData.Activities.PlayLog_Trnmnt = (byte)numPlayLog_Trnmnt.Value;
            save.SaveData.Activities.PlayLog_Sing = (byte)numPlayLog_Sing.Value;
            save.SaveData.Activities.PlayLog_Lunch = (byte)numPlayLog_Lunch.Value;
            save.SaveData.Activities.PlayLog_Cooking = (byte)numPlayLog_Cooking.Value;
            save.SaveData.Activities.PlayLog_Drill = (byte)numPlayLog_Drill.Value;
            save.SaveData.Activities.PlayLog_Teaparty = (byte)numPlayLog_Teaparty.Value;
            save.SaveData.Activities.PlayLog_SCOUT = (byte)numPlayLog_SCOUT.Value;

            save.SaveData.Activities.ActivityExplore = (byte)numActivityExplore.Value;
            save.SaveData.Activities.ActivityLesson = (byte)numActivityLesson.Value;
            save.SaveData.Activities.ActivityBattle = (byte)numActivityBattle.Value;

            save.SaveData.Activities.Statue1 = (byte) numStatue1.Value;
            save.SaveData.Activities.Statue2 = (byte) numStatue2.Value;
            save.SaveData.Activities.Statue3 = (byte) numStatue3.Value;
            save.SaveData.Activities.Statue4 = (byte) numStatue4.Value;
        }

        private void LoadItems(int index = 0)
        {
            if(save == null)
                return;

            lstItems.Items.Clear();
            
            for (int i = 0; i < save.SaveData.Items.Length; i++)
                lstItems.Items.Add($"[{i:D3}] - {save.SaveData.Items[i]}");

            lstItems.SelectedIndex = index;

            grpItemList.Text = $@"Item List {save.SaveData.ItemCount} / {SaveData_1000.ITEM_COUNT}";
        }

        private void LoadItems2()
        {
            if(save == null)
                return;

            lsvMiscItems.Items.Clear();
            lsvGiftItems.Items.Clear();

            for (int i = 0; i < save.SaveData.Player.MiscItems.Length; i++)
            {
                var entry = new ListViewItem {Text = $@"{i:D3} - {Database.GetMiscItemName(i)}", Tag = i.ToString()};
                entry.SubItems.Add(save.SaveData.Player.MiscItems[i].ToString());

                lsvMiscItems.Items.Add(entry);
            }

            for (int i = 0; i < save.SaveData.Player.GiftItems.Length; i++)
            {
                var entry = new ListViewItem {Text = $@"{i:D3} - {Database.GetGiftItemName(i)}", Tag = i.ToString()};
                entry.SubItems.Add(save.SaveData.Player.GiftItems[i].ToString());

                lsvGiftItems.Items.Add(entry);
            }
        }

        private void SaveItem()
        {
            int x = lstItems.SelectedIndex;
			
            if(x == -1 || save == null)
                return;

            Item item = new Item { Id = (short)Util.GetSortedKey<string>(cboItemEditor) };
            item.Durability = item.Id > -1 ? (byte) numItemDurability.Value : (byte) 0;
            item.Amount = item.Id > -1 ? (byte) numItemAmount.Value : (byte) 0;
            
            save.SaveData.Items[x] = item;

            UpdateItemCount();

            //sorting is needed, because game only reads up to the item count
            btnSortItems_Click(null, null);
        }

        private void LoadCharacters(int index = 0)
        {            
            if(save == null)
                return;

            lstCharacter.Items.Clear();
                        
            for (int i = 0; i < save.SaveData.Characters.Length; i++)
                lstCharacter.Items.Add($"[{i:D3}] - {save.SaveData.Characters[i]}");

            lstCharacter.SelectedIndex = index;
        }

        private void LoadCharacterClassExp()
        {
            int x = lstCharacter.SelectedIndex;

            if(x == -1 || save == null)
                return;

            CharacterData chara = save.SaveData.Characters[x].data;

            lsvClassExp.Items.Clear();

            for (int i = 0; i < Database.MAX_CLASS; i++)
            {
                var entry = new ListViewItem {Text = Database.GetClassName(i), Tag = i.ToString()};
                entry.SubItems.Add(chara.ClassExp[i].ToString());
                entry.SubItems.Add(chara.ClassLevel[i].ToString());
                lsvClassExp.Items.Add(entry);
            }
            
            SetListViewIndex(lsvClassExp, save.SaveData.Characters[x].Class);
        }

		private void SaveCharacter()
		{
			int x = lstCharacter.SelectedIndex;
			
            if(x == -1 || save == null)
                return;
			
			CharacterData chara = save.SaveData.Characters[x].data;

            chara.Id = (short)Util.GetSortedKey<string>(cboCharaId);
            chara.RNG_VALUE = (uint) numRNG.Value;
            chara.Level = (byte)numCharaLevel.Value;
            chara.Exp = (ushort)numCharaExp.Value;

            chara.SkillExp[0] = (ushort)numCharaSword.Value;
            chara.SkillExp[1] = (ushort)numCharaLance.Value;
            chara.SkillExp[2] = (ushort)numCharaAxe.Value;
            chara.SkillExp[3] = (ushort)numCharaBow.Value;
            chara.SkillExp[4] = (ushort)numCharaFighting.Value;
            chara.SkillExp[5] = (ushort)numCharaReason.Value;
            chara.SkillExp[6] = (ushort)numCharaFaith.Value;
            chara.SkillExp[7] = (ushort)numCharaAuthority.Value;
            chara.SkillExp[8] = (ushort)numCharaArmor.Value;
            chara.SkillExp[9] = (ushort)numCharaRiding.Value;
            chara.SkillExp[10] = (ushort)numCharaFly.Value;

            chara.CurrentClassExp = (ushort)numCurrentClassExp.Value;

            chara.HP = (byte)numCharaHP.Value;
            chara.Strength = (byte)numCharaStrength.Value;
            chara.Magic = (byte)numCharaMagic.Value;
            chara.Dexterity = (byte)numCharaDexterity.Value;
            chara.Speed = (byte)numCharaSpeed.Value;
            chara.Luck = (byte)numCharaLuck.Value;
            chara.Defense = (byte)numCharaDefense.Value;
            chara.Resistance = (byte)numCharaResistance.Value;
            chara.Movement = (byte)numCharaMovement.Value;
            chara.Charm = (byte)numCharaCharm.Value;

            chara.CombatArts = Util.GetFlagTableArray(chkCombatArts);
            chara.Abilities = Util.GetFlagTableArray(chkAbilities);

            chara.EquippedAbilities[0] = (byte)Util.GetSortedKey<string>(cboAbility1);
            chara.EquippedAbilities[1] = (byte)Util.GetSortedKey<string>(cboAbility2);
            chara.EquippedAbilities[2] = (byte)Util.GetSortedKey<string>(cboAbility3);
            chara.EquippedAbilities[3] = (byte)Util.GetSortedKey<string>(cboAbility4);
            chara.EquippedAbilities[4] = (byte)Util.GetSortedKey<string>(cboAbility5);

            chara.EquippedCombatArts[0] = (byte)Util.GetSortedKey<string>(cboBattleSkill1);
            chara.EquippedCombatArts[1] = (byte)Util.GetSortedKey<string>(cboBattleSkill2);
            chara.EquippedCombatArts[2] = (byte)Util.GetSortedKey<string>(cboBattleSkill3);

            chara.CurrentClassLevel = (byte)numCurrentClassLevel.Value;

            chara.Flags = Util.GetFlagTableUint(chkCharaFlags);
            chara.Motivation = (byte)numCharaMotivation.Value;
            chara.ClassUnlockFlags = Util.GetFlagTableArray(chkCharaClassUnlockFlags);
            chara.ClassFlags = Util.GetFlagTableByte(chkCharaClassFlags);

            chara.AdjutantId = (short)Util.GetSortedKey<string>(cboAdjutant);

            chara.SkillExp2[0] = (ushort)numCharaSword.Value;
            chara.SkillExp2[1] = (ushort)numCharaLance.Value;
            chara.SkillExp2[2] = (ushort)numCharaAxe.Value;
            chara.SkillExp2[3] = (ushort)numCharaBow.Value;
            chara.SkillExp2[4] = (ushort)numCharaFighting.Value;
            chara.SkillExp2[5] = (ushort)numCharaReason.Value;
            chara.SkillExp2[6] = (ushort)numCharaFaith.Value;
            chara.SkillExp2[7] = (ushort)numCharaAuthority.Value;
            chara.SkillExp2[8] = (ushort)numCharaArmor.Value;
            chara.SkillExp2[9] = (ushort)numCharaRiding.Value;
            chara.SkillExp2[10] = (ushort)numCharaFly.Value;
			                        
            save.SaveData.Characters[x].data = chara;

            LoadCharacters(x);
        }

        private void LoadBattalions(int index = 0)
        {
            if(save == null)
                return;
            
            lstBattalion.Items.Clear();
            			                        
            for (int i = 0; i < save.SaveData.Player.Battalions.Length; i++)
                lstBattalion.Items.Add($"[{i:D3}] - {save.SaveData.Player.Battalions[i].GetBarracksName()}");

            lstBattalion.SelectedIndex = index;
        }

        private void SaveBattalion()
        {
            int x = lstBattalion.SelectedIndex;
			
            if(x == -1 || save == null)
                return;

            short id;
            try
            {
                id = (short) Util.GetSortedKey<string>(cboBattalionCharacter);
            }
            catch (Exception)
            {

                id = -1;
            }
            
            Battalion battalion = new Battalion
            {
                CharacterId = id,
                Type = (byte)Util.GetSortedKey<string>(cboBattalionType),
                Exp = (ushort)numBattalionExp.Value,
                Stamina = (ushort)numBattalionStamina.Value,
                Skill = (byte)Util.GetSortedKey<string>(cboBattalionSkill)
            };
                        			                        
            save.SaveData.Player.Battalions[x] = battalion;

            LoadBattalions(x);
        }
        
        private void LoadDatabaseViewer()
        {
            lstCharacterDB.Items.Clear();
            lstClassDB.Items.Clear();
            lstItemDB.Items.Clear();

            for (int i = 0; i < Database.BinaryDatabase.CharacterEntries.Count; i++)
            {
                //var entry = Database.BinaryDatabase.CharacterEntries[i];
                lstCharacterDB.Items.Add($@"[{i:D4}] {Database.GetUnitName(i, ShowGender:true)}");
            }
            
            for (int i = 0; i < Database.BinaryDatabase.ClassEntries.Count; i++)
            {
                //var entry = Database.BinaryDatabase.ClassEntries[i];
                lstClassDB.Items.Add($@"[{i:D2}] {Database.GetClassName(i)}");
            }

            foreach (var entry in Database.BinaryDatabase.ItemEntries)
            {
                lstItemDB.Items.Add($@"[{entry.Key:D4}] {Database.GetItemName(entry.Key)}");
            }
        }

		#endregion

		#region list box logic
        
        private void lstStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstItems.SelectedIndex;

            if(x == -1 || save == null)
                return;

            Item item = save.SaveData.Items[x];

            Util.SetSortedIndex<string>(cboItemEditor, item.Id);
            numItemDurability.Value = item.Durability;
            numItemAmount.Value = item.Amount;
        }

        private void lstCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(x == -1 || save == null)
                return;
              
            CharacterData chara = save.SaveData.Characters[x].data;

            lstCharacterItems.Items.Clear();
            for (int i = 0; i < Database.MAX_CHARA_ITEMS; i++) lstCharacterItems.Items.Add(chara.Items[i].EquippedName);
            for (int i = 0; i < chkCharaFlags.Items.Count; i++) chkCharaFlags.SetItemChecked(i, false);
            for (int i = 0; i < chkCharaClassFlags.Items.Count; i++) chkCharaClassFlags.SetItemChecked(i, false);
            for (int i = 0; i < chkCharaClassUnlockFlags.Items.Count; i++) chkCharaClassUnlockFlags.SetItemChecked(i, false);
            for (int i = 0; i < chkAbilities.Items.Count; i++) chkAbilities.SetItemChecked(i, false);
            for (int i = 0; i < chkCombatArts.Items.Count; i++) chkCombatArts.SetItemChecked(i, false);

            txtCharacterDebug.Text = "";
            
            Util.SetSortedIndex<string>(cboCharaId, chara.Id);

            if(chara.EquippedBattalion.CharacterId >= Database.CHARACTER_USEABLE_COUNT)
                Util.SetSortedIndex<string>(cboCharaBattalion, -1);
            else
                Util.SetSortedIndex<string>(cboCharaBattalion, chara.EquippedBattalion.CharacterId);

            numRNG.Value = chara.RNG_VALUE;

            //txtCharacterDebug.Text += $"field_26: {chara1.field_26}, field_28: {chara1.field_28}, field_2A: {chara1.field_2A}, field_2B: {chara1.field_2B}\r\n";
            
            numCharaExp.Value = chara.Exp;

            //txtCharacterDebug.Text += $"EquippedItems: {chara1.EquippedItem[0]}, {chara1.EquippedItem[1]}\r\n";
            
            numCharaSword.Value = chara.SkillExp[0];
            numCharaLance.Value = chara.SkillExp[1];
            numCharaAxe.Value = chara.SkillExp[2];
            numCharaBow.Value = chara.SkillExp[3];
            numCharaFighting.Value = chara.SkillExp[4];
            numCharaReason.Value = chara.SkillExp[5];
            numCharaFaith.Value = chara.SkillExp[6];
            numCharaAuthority.Value = chara.SkillExp[7];
            numCharaArmor.Value = chara.SkillExp[8];
            numCharaRiding.Value = chara.SkillExp[9];
            numCharaFly.Value = chara.SkillExp[10];
            numCurrentClassExp.Value = chara.CurrentClassExp;
            numCharaLevel.Value = chara.Level;
            Util.SetSortedIndex<string>(cboClass, chara.Class);


            //stats
            numCharaHP.Maximum = Database.GetMaxHP(chara.Id, chara.Class);
            numCharaStrength.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 0);
            numCharaMagic.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 1);
            numCharaDexterity.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 2);
            numCharaSpeed.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 3);
            numCharaLuck.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 4);
            numCharaDefense.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 5);
            numCharaResistance.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 6);
            numCharaMovement.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 7);
            numCharaCharm.Maximum = Database.GetMaxStat(chara.Id, chara.Class, 8);

            numCharaHP.Value = Math.Min(chara.HP, Database.GetMaxHP(chara.Id, chara.Class));
            //txtCharacterDebug.Text += $"field_4D: {chara1.field_4D}\r\n";
            numCharaStrength.Value = Math.Min(chara.Strength, Database.GetMaxStat(chara.Id, chara.Class, 0));
            numCharaMagic.Value = Math.Min(chara.Magic, Database.GetMaxStat(chara.Id, chara.Class, 1));
            numCharaDexterity.Value = Math.Min(chara.Dexterity, Database.GetMaxStat(chara.Id, chara.Class, 2));
            numCharaSpeed.Value = Math.Min(chara.Speed, Database.GetMaxStat(chara.Id, chara.Class, 3));
            numCharaLuck.Value = Math.Min(chara.Luck, Database.GetMaxStat(chara.Id, chara.Class, 4));
            numCharaDefense.Value = Math.Min(chara.Defense, Database.GetMaxStat(chara.Id, chara.Class, 5));
            numCharaResistance.Value = Math.Min(chara.Resistance, Database.GetMaxStat(chara.Id, chara.Class, 6));
            numCharaMovement.Value = Math.Min(chara.Movement, Database.GetMaxStat(chara.Id, chara.Class, 7));
            numCharaCharm.Value = Math.Min(chara.Charm, Database.GetMaxStat(chara.Id, chara.Class, 8));

            Util.FillFlagTableArray(chkCombatArts, chara.CombatArts);
            Util.FillFlagTableArray(chkAbilities, chara.Abilities);

            Util.SetSortedIndex<string>(cboAbility1, chara.EquippedAbilities[0]);
            Util.SetSortedIndex<string>(cboAbility2, chara.EquippedAbilities[1]);
            Util.SetSortedIndex<string>(cboAbility3, chara.EquippedAbilities[2]);
            Util.SetSortedIndex<string>(cboAbility4, chara.EquippedAbilities[3]);
            Util.SetSortedIndex<string>(cboAbility5, chara.EquippedAbilities[4]);

            Util.SetSortedIndex<string>(cboBattleSkill1, chara.EquippedCombatArts[0]);
            Util.SetSortedIndex<string>(cboBattleSkill2, chara.EquippedCombatArts[1]);
            Util.SetSortedIndex<string>(cboBattleSkill3, chara.EquippedCombatArts[2]);

            grpCharaItemList.Text = $@"Item List {chara.ItemCount} / 6";

            lblCharaSword.Text = GetSkillRankLabel(chara.SkillLevel[0]);
            lblCharaLance.Text = GetSkillRankLabel(chara.SkillLevel[1]);
            lblCharaAxe.Text = GetSkillRankLabel(chara.SkillLevel[2]);
            lblCharaBow.Text = GetSkillRankLabel(chara.SkillLevel[3]);
            lblCharaFighting.Text = GetSkillRankLabel(chara.SkillLevel[4]);
            lblCharaReason.Text = GetSkillRankLabel(chara.SkillLevel[5]);
            lblCharaFaith.Text = GetSkillRankLabel(chara.SkillLevel[6]);
            lblCharaAuthority.Text = GetSkillRankLabel(chara.SkillLevel[7]);
            lblCharaArmor.Text = GetSkillRankLabel(chara.SkillLevel[8]);
            lblCharaRiding.Text = GetSkillRankLabel(chara.SkillLevel[9]);
            lblCharaFly.Text = GetSkillRankLabel(chara.SkillLevel[10]);

            numCurrentClassLevel.Value = (byte)Math.Min((int)chara.CurrentClassLevel, Database.MAX_CLASS_LEVEL);

            lsvCharaMagic.Items.Clear();
            
            for (int i = 0; i < Database.MAX_MAGIC; i++)
            {
                var entry = new ListViewItem {Text = i.ToString("D2"), Tag = i.ToString()};

                entry.SubItems.Add(Database.GetMagicSkillName(chara.LearnedMagic[i]));
                entry.SubItems.Add(chara.LearnedMagic[i].ToString());
                entry.SubItems.Add(chara.MagicDurability[i].ToString());

                lsvCharaMagic.Items.Add(entry);
            }

            //txtCharacterDebug.Text += $"MagicDurability: {Util.Array2String(chara1.MagicDurability, " ")}\r\n";
            //txtCharacterDebug.Text += $"LearnedMagic: {Util.Array2String(chara1.LearnedMagic, " ")}\r\n";
            
            Util.SetFlagTableUint(chkCharaFlags, chara.Flags);

            //txtCharacterDebug.Text += $"field_B0: {Util.Array2String(chara1.field_B0, " ")}\r\n";

            numCharaMotivation.Value = chara.Motivation;
                 
            //txtCharacterDebug.Text += $"field_C5: {Util.Array2String(chara1.field_C5, " ")}\r\n";
            //txtCharacterDebug.Text += $"field_D0: {Util.Array2String(chara1.field_D0, " ")}\r\n";

            Util.FillFlagTableArray(chkCharaClassUnlockFlags, chara.ClassUnlockFlags);

            //txtCharacterDebug.Text += $"LearningFocus: {chara1.LearningFocus}\r\n";
            //txtCharacterDebug.Text += $"field_DC: {Util.Array2String(chara1.field_DC, " ")}\r\n";

            Util.SetFlagTableByte(chkCharaClassFlags, chara.ClassFlags);

            //txtCharacterDebug.Text += $"field_E0: {Util.Array2String(chara1.field_E0, " ")}\r\n";
            //txtCharacterDebug.Text += $"field_EA: {Util.Array2String(chara1.field_EA, " ")}\r\n";
            //txtCharacterDebug.Text += $"field_F4: {Util.Array2String(chara1.field_F4, " ")}\r\n";

            if(chara.AdjutantId >= Database.CHARACTER_USEABLE_COUNT)
                Util.SetSortedIndex<string>(cboAdjutant, -1);
            else
                Util.SetSortedIndex<string>(cboAdjutant, chara.AdjutantId);
            
            LoadCharacterClassExp(); //update exp, this also updates the focus...
            lstCharacter.Focus(); //fix to prevent losing the focus (no focus == arrow keys no longer working)

            //txtCharacterDebug.Text += $"field_18A: {Util.Array2String(chara1.field_18A, " ")}\r\n";

            txtCharacterDebug.Text += chara.GenerateDebugOut();
        }
        
        private void lstBattalion_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstBattalion.SelectedIndex;

            if(x == -1 || save == null)
                return;
            
            Battalion battalion = save.SaveData.Player.Battalions[x];

            Util.SetSortedIndex<string>(cboBattalionCharacter, battalion.CharacterId);
            Util.SetSortedIndex<string>(cboBattalionType, battalion.Type == Database.BATTALION_COUNT ? -1 : battalion.Type);
            numBattalionExp.Value = battalion.Exp;
            numBattalionStamina.Value = battalion.Stamina;
            Util.SetSortedIndex<string>(cboBattalionSkill, battalion.Skill);
        }

        private void lsvJobExp_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(x == -1 || lsvClassExp.SelectedItems.Count == 0 || save == null)
                return;
            
            var job = lsvClassExp.SelectedItems[0];

            int id = int.Parse(job.Tag.ToString());
            
            numSetClassExp.Value = save.SaveData.Characters[x].data.ClassExp[id];
            numSetClassLevel.Value = save.SaveData.Characters[x].data.ClassLevel[id];
        }

        private void lsvMiscItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsvMiscItems.SelectedItems.Count == 0 || save == null)
                return;
            
            var item = lsvMiscItems.SelectedItems[0];

            int id = int.Parse(item.Tag.ToString());
                      
            numEditMiscItem.Value = save.SaveData.Player.MiscItems[id];
        }

        private void lsvGiftItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsvGiftItems.SelectedItems.Count == 0 || save == null)
                return;
            
            var item = lsvGiftItems.SelectedItems[0];

            int id = int.Parse(item.Tag.ToString());
                                  
            numEditGiftItem.Value = save.SaveData.Player.GiftItems[id];
        }
        
        private void lsvQuest_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsvQuest.SelectedItems.Count == 0 || save == null)
                return;
                
            var item = lsvQuest.SelectedItems[0];

            int id = int.Parse(item.Tag.ToString());
                                  
            cboQuestState.SelectedIndex = save.SaveData.Activities.QuestStateList[id];
        }
        
        private void lsvTalk_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsvTalk.SelectedItems.Count == 0 || save == null)
                return;
                
            var item = lsvTalk.SelectedItems[0];

            int id = int.Parse(item.Tag.ToString());
                                              
            numTalkSetState.Value = save.SaveData.Player.CharacterSupportValues[id];
        }

        private void SkillLevel_ValueChanged(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(IsUpdating || x == -1 || save == null)
                return;
            
            IsUpdating = true;

            CharacterData chara = save.SaveData.Characters[x].data;

            numCharaSword.Value = GetValidSkillExp((int)numCharaSword.Value, chara.SkillLevel[0]);
            numCharaLance.Value = GetValidSkillExp((int)numCharaLance.Value, chara.SkillLevel[1]);
            numCharaAxe.Value = GetValidSkillExp((int)numCharaAxe.Value, chara.SkillLevel[2]);
            numCharaBow.Value = GetValidSkillExp((int)numCharaBow.Value, chara.SkillLevel[3]);
            numCharaFighting.Value = GetValidSkillExp((int)numCharaFighting.Value, chara.SkillLevel[4]);
            numCharaReason.Value = GetValidSkillExp((int)numCharaReason.Value, chara.SkillLevel[5]);
            numCharaFaith.Value = GetValidSkillExp((int)numCharaFaith.Value, chara.SkillLevel[6]);
            numCharaAuthority.Value = GetValidSkillExp((int)numCharaAuthority.Value, chara.SkillLevel[7]);
            numCharaArmor.Value = GetValidSkillExp((int)numCharaArmor.Value, chara.SkillLevel[8]);
            numCharaRiding.Value = GetValidSkillExp((int)numCharaRiding.Value, chara.SkillLevel[9]);
            numCharaFly.Value = GetValidSkillExp((int)numCharaFly.Value, chara.SkillLevel[10]);

            IsUpdating = false;
        }
        
        private void CurrentClass_ValueChanged(object sender, EventArgs e)
        {
            int x = lstCharacter.SelectedIndex;

            if(IsUpdating || x == -1 || save == null)
                return;
            
            IsUpdating = true;

            int class_id= save.SaveData.Characters[x].Class;
            ushort exp = (ushort) numCurrentClassExp.Value;
            byte level = (byte)Math.Min((int)numCurrentClassLevel.Value, Database.MAX_CLASS_LEVEL);

            save.SaveData.Characters[x].data.CurrentClassExp = (ushort)Math.Min(exp, Database.GetMaxClassExp(class_id));
            save.SaveData.Characters[x].data.CurrentClassLevel = level;
            save.SaveData.Characters[x].data.ClassExp[class_id] = (ushort)Math.Min(exp, Database.GetMaxClassExp(class_id));
            save.SaveData.Characters[x].data.ClassLevel[class_id] = level;

            IsUpdating = false;
 
            LoadCharacterClassExp();
        }
        
        private void lstCharacterDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstCharacterDB.SelectedIndex;

            if(x == -1)
                return;

            txtCharacterDBDebug.Text = Database.BinaryDatabase.CharacterEntries[x].GenerateDebugOut();
        }

        private void lstClassDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstClassDB.SelectedIndex;

            if(x == -1)
                return;

            txtClassDBDebug.Text = Database.BinaryDatabase.ClassEntries[x].GenerateDebugOut();
        }
        
        private void lstItemDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int x = lstItemDB.SelectedIndex;

            if(x == -1)
                return;

            txtItemDBDebug.Text = Database.BinaryDatabase.ItemEntries.ElementAt(x).Value.GenerateDebugOut();
        }

		#endregion

        #region Functions
        
        private int GetValidSkillExp(int value, int level)
        {
            return Math.Min(value, GetMaxSkillExp(level));
        }
            
        private int GetMaxSkillExp(int level)
        {
            return Database.SkillLevelupRank[level] - 1;
        }

        private string GetSkillRankLabel(int level)
        {
            int exp = Database.SkillLevelupRank[level];
            return $@"/ {exp} == {((enmRank)level).GetDescription()}";
        }
        
        private void SetListViewIndex(ListView lsv, int index = -1)
        {
            if (index != -1)
            {
                lsv.FocusedItem = lsv.Items[index];
                lsv.Items[index].Selected = true;
                lsv.Select();
                lsv.EnsureVisible(index);
            }
        }
        
        private int DoesItemExist(int id)
        {
            if(save == null)
                return -1;
            
            for (int i = 0; i < save.SaveData.Items.Length; i++)
            {
                var item = save.SaveData.Items[i];

                if (item.Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetFreeItemSlot()
        {
            if(save == null)
                return -1;
                        
            for (int i = 0; i < save.SaveData.Items.Length; i++)
            {
                var item = save.SaveData.Items[i];

                if (item.Id == -1)
                {
                    return i;
                }
            }

            return -1;
        }
        
        private void AddItem(short id)
        {
            if(save == null)
                return;

            var index = DoesItemExist(id);
                                   
            if (index == -1) //add new item
            {
                int freeSlot = GetFreeItemSlot();

                if (freeSlot == -1) return;

                save.SaveData.Items[freeSlot] = new Item
                {
                    Id = id,
                    Durability = (byte)Database.GetItemDurability(id),
                    Amount = 99
                };
            }
            else //max item
            {
                var item = save.SaveData.Items[index];

                item.Durability = (byte)Database.GetItemDurability(item.Id);
                item.Amount = 99;

                save.SaveData.Items[index] = item;
            }
        }

        private void UpdateItemCount()
        {
            //calculate new item count
            save.SaveData.ItemCount = 0;
            for (int i = 0; i < save.SaveData.Items.Length; i++)
            {
                if(save.SaveData.Items[i].Id == -1) continue;
                save.SaveData.ItemCount++;
            }
        }

        #endregion

        



    }
}
