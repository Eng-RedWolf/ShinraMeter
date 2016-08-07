﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Data;
using Lang;
using Tera.Game;

namespace DamageMeter.UI
{
    /// <summary>
    /// Logique d'interaction pour SkillLog.xaml
    /// </summary>
    public partial class SkillLog
    {
        public SkillLog()
        {
            InitializeComponent();
        }


        public void Update(Database.Structures.Skill skill, bool received)
        {
            var skillInfo = SkillResult.GetSkill(skill.Source, skill.Pet, skill.SkillId, skill.HotDot,
                           NetworkController.Instance.EntityTracker, BasicTeraData.Instance.SkillDatabase,
                           BasicTeraData.Instance.HotDotDatabase, BasicTeraData.Instance.PetSkillDatabase);
            var entity = NetworkController.Instance.EntityTracker.GetOrNull(received ? skill.Source : skill.Target);
            Brush color = null;
            var fontWeight = FontWeights.Normal;
            if (skill.Critic)
            {
                fontWeight = FontWeights.Bold;
            }
            switch (skill.Type)
            {
                case Database.Database.Type.Damage:
                    color = Brushes.Red;
                    break;
                case Database.Database.Type.Heal:
                    color = Brushes.LawnGreen;
                    break;
                case Database.Database.Type.Mana:
                    color = Brushes.DeepSkyBlue;
                    break;
            }

            SkillAmount.Foreground = color;
            SkillAmount.FontWeight = fontWeight;
            SkillAmount.ToolTip = skill.Critic ? LP.Critical :  LP.White;
            SkillName.Content = LP.Unknown_Skill;
            if (skillInfo != null)
            {
                SkillIcon.Source = BasicTeraData.Instance.Icons.GetImage(skillInfo.IconName);
                SkillName.Content = skillInfo.Name;
            }
            SkillAmount.Content = skill.Amount;
            SkillIcon.ToolTip = skill.SkillId;
            SkillDirection.Content = skill.Direction.ToString();
            switch (skill.Direction)
            {
                    case HitDirection.Back:
                        SkillDirection.Foreground = Brushes.Red;
                    break;
                    case HitDirection.Dot:
                    break;
                    case HitDirection.Front:
                    SkillDirection.Foreground = Brushes.BlueViolet;
                    break;
                    case HitDirection.Side:
                    SkillDirection.Foreground = Brushes.SpringGreen;
                    break;

            }

            SkillName.ToolTip = skill.Time;
            if (entity is NpcEntity)
            {
                var npc = (NpcEntity) entity;
                SkillTarget.Content = npc.Info.Name + " : " + npc.Info.Area;
            }else if (entity is UserEntity)
            {
                SkillTarget.Content = ((UserEntity) entity).Name ;
            }
            SkillPet.Content = skill.Pet == null ? "" : skill.Pet.Name;
                
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var w = Window.GetWindow(this);
                w?.DragMove();
            }
            catch
            {
                Console.WriteLine(@"Exception move");
            }
        }
    }
}