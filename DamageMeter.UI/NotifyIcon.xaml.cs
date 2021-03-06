﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DamageMeter.AutoUpdate;
using Data;
using Data.Actions.Notify;
using Lang;
using Newtonsoft.Json;

namespace DamageMeter.UI
{
    /// <summary>
    ///     Logique d'interaction pour NotifyIcon.xaml
    /// </summary>
    public partial class NotifyIcon
    {
        private long _lastSend;

        private MainWindow _mainWindow;

        public NotifyIcon()
        {
            InitializeComponent();
            Tray.ToolTipText = "Shinra Meter V" + UpdateManager.Version + ": " + LP.SystemTray_No_server;
        }

        public void UpdatePacketWaiting(int packetWaiting)
        {
            PacketWaitingLabel.Content = $"{packetWaiting} /3000 {LP.SystemTray_before_crash}";
            PacketWaitingProgressBar.Value = packetWaiting;
        }

        public void Initialize(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            DpsWebsiteExport.IsChecked = BasicTeraData.Instance.WindowData.SiteExport;
            AuthTokenTextbox.Text = BasicTeraData.Instance.WindowData.TeraDpsToken;
            AuthTokenTextbox.Parent.SetValue(HeightProperty, BasicTeraData.Instance.WindowData.SiteExport ? double.NaN : 0);
            AutoExcelExport.IsChecked = BasicTeraData.Instance.WindowData.Excel;
            ExcelCMADPSSpinner.Value = BasicTeraData.Instance.WindowData.ExcelCMADPSSeconds;
            CountOnlyBoss.IsChecked = BasicTeraData.Instance.WindowData.OnlyBoss;
            BossByHpBar.IsChecked = BasicTeraData.Instance.WindowData.DetectBosses;
            PartyOnly.IsChecked = BasicTeraData.Instance.WindowData.PartyOnly;
            InvisibleWhenNoStats.IsChecked = BasicTeraData.Instance.WindowData.InvisibleUi;
            ShowAlways.IsChecked = BasicTeraData.Instance.WindowData.AlwaysVisible;
            StayTopMost.IsChecked = BasicTeraData.Instance.WindowData.Topmost;
            NumberPlayersSpinner.Value = BasicTeraData.Instance.WindowData.NumberOfPlayersDisplayed;
            LFDelaySpinner.Value = BasicTeraData.Instance.WindowData.LFDelay;
            RemoveTeraAltEnterHotkey.IsChecked = BasicTeraData.Instance.WindowData.RemoveTeraAltEnterHotkey;
            ChatEnabled.IsChecked = BasicTeraData.Instance.WindowData.EnableChat;
            CopyInspect.IsChecked = BasicTeraData.Instance.WindowData.CopyInspect;
            FormatPasteString.IsChecked = BasicTeraData.Instance.WindowData.FormatPasteString;
            SayColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.SayColor;
            GroupColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.GroupColor;
            AllianceColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.AllianceColor;
            AreaColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.AreaColor;
            WhisperColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.WhisperColor;
            GuildColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.GuildColor;
            EmotesColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.SayColor;
            TradingColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.TradingColor;
            PrivateChannelColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.PrivateChannelColor;
            GeneralColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.GeneralColor;
            RaidColorSelecter.SelectedColor = BasicTeraData.Instance.WindowData.RaidColor;
            PartyEvent.IsChecked = BasicTeraData.Instance.WindowData.DisablePartyEvent;
            ShowAfkIventsIngame.IsChecked = BasicTeraData.Instance.WindowData.ShowAfkEventsIngame;
            PrivateServerExport.IsChecked = BasicTeraData.Instance.WindowData.PrivateServerExport;
            ServerURLTextbox.Text = BasicTeraData.Instance.WindowData.PrivateDpsServers[0];
            MuteSound.IsChecked = BasicTeraData.Instance.WindowData.MuteSound;
            ShowSelfOnTop.IsChecked = BasicTeraData.Instance.WindowData.MeterUserOnTop;
            IdleRTOSpinner.Value = BasicTeraData.Instance.WindowData.IdleResetTimeout;
            NoPaste.IsChecked = BasicTeraData.Instance.WindowData.NoPaste;
            ChatSettingsVisible(BasicTeraData.Instance.WindowData.EnableChat);
            ServerURLTextbox.Parent.SetValue(HeightProperty, BasicTeraData.Instance.WindowData.PrivateServerExport ? double.NaN : 0);
        }


        public void ShowBallon(NotifyFlashMessage flash)
        {
            if (flash == null) { return; }

            Tray.HideBalloonTip();
            if (flash.Balloon != null && flash.Balloon.DisplayTime >= 500)
            {
                var balloon = new Balloon();
                balloon.Value(flash.Balloon.TitleText, flash.Balloon.BodyText);
                Tray.ShowCustomBalloon(balloon, PopupAnimation.Fade, flash.Balloon.DisplayTime);
            }

            if (!BasicTeraData.Instance.WindowData.MuteSound && flash.Sound != null) { Task.Run(() => flash.Sound.Play()); }
        }

        private void ResetAction(object sender, RoutedEventArgs e)
        {
            NetworkController.Instance.NeedToReset = true;
        }

        private void CloseAction(object sender, RoutedEventArgs e)
        {
            _mainWindow.VerifyClose();
        }

        private void WikiAction(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/neowutran/ShinraMeter/wiki");
        }

        private void PatchAction(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/neowutran/ShinraMeter/wiki/Patch-note");
        }

        private void IssueAction(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://github.com/neowutran/ShinraMeter/issues");
        }

        private void DiscordAction(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "https://discord.gg/anUXQTp");
        }

        public void SwitchStayTop(object sender = null, EventArgs e = null)
        {
            BasicTeraData.Instance.WindowData.Topmost = !BasicTeraData.Instance.WindowData.Topmost;
            StayTopMost.IsChecked = BasicTeraData.Instance.WindowData.Topmost;
            UpdateTopMost();
        }

        private void DpsWebsiteAction(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "http://moongourd.com");
        }

        private void EnableDpsWebsiteExportAction(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.SiteExport = true;
            AuthTokenTextbox.Parent.SetValue(HeightProperty, BasicTeraData.Instance.WindowData.SiteExport ? double.NaN : 0);
        }

        private void DisableDpsWebsiteExportAction(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.SiteExport = false;
            AuthTokenTextbox.Parent.SetValue(HeightProperty, BasicTeraData.Instance.WindowData.SiteExport ? double.NaN : 0);
        }

        private void ExcelExportAction(object sender, RoutedEventArgs e)
        {
            NetworkController.Instance.NeedToExport = DataExporter.Dest.Excel | DataExporter.Dest.Manual;
        }

        private void SiteExportAction(object sender, RoutedEventArgs e)
        {
            if (_lastSend + TimeSpan.TicksPerSecond * 60 >= DateTime.Now.Ticks) { return; }
            NetworkController.Instance.NeedToExport = DataExporter.Dest.Site;
            _lastSend = DateTime.Now.Ticks;
        }

        private void EnableAutoExcelExportAction(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.Excel = true;
        }

        private void DisableExcelExportAction(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.Excel = false;
        }

        private void DisableStayTopMost(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.Topmost = false;
            UpdateTopMost();
        }

        private void EnableStayTopMost(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.Topmost = true;
            UpdateTopMost();
        }

        private void UpdateTopMost()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Topmost = BasicTeraData.Instance.WindowData.Topmost;
                window.ShowInTaskbar = !BasicTeraData.Instance.WindowData.Topmost;
            }
        }

        private void EnableShowAlways(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.AlwaysVisible = true;
        }

        private void DisableShowAlways(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.AlwaysVisible = false;
        }

        private void EnableInvisibleWhenNoStats(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.InvisibleUi = true;
            if (_mainWindow.ForceWindowVisibilityHidden) { return; }
            _mainWindow.Visibility = Visibility.Visible;
        }

        private void DisableInvisibleWhenNoStats(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.InvisibleUi = false;
            if (_mainWindow.ForceWindowVisibilityHidden) { return; }
            _mainWindow.Visibility = _mainWindow.Controls.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void EnableClickThrou(object sender, RoutedEventArgs e)
        {
            NetworkController.Instance.SwitchClickThrou(true);
        }

        private void DisableClickThrou(object sender, RoutedEventArgs e)
        {
            NetworkController.Instance.SwitchClickThrou(false);
        }

        private void EnablePartyOnly(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.PartyOnly = true;
        }

        private void DisablePartyOnly(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.PartyOnly = false;
        }

        private void EnableBossByHpBar(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.DetectBosses = true;
            if (BasicTeraData.Instance.MonsterDatabase != null) { BasicTeraData.Instance.MonsterDatabase.DetectBosses = true; }
        }

        private void DisableBossByHpBar(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.DetectBosses = false;
            if (BasicTeraData.Instance.MonsterDatabase != null) { BasicTeraData.Instance.MonsterDatabase.DetectBosses = false; }
        }

        private void EnableCountOnlyBoss(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.OnlyBoss = true;
        }

        private void DisableCountOnlyBoss(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.OnlyBoss = false;
        }

        private void NumberPlayersChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BasicTeraData.Instance.WindowData.NumberOfPlayersDisplayed = NumberPlayersSpinner?.Value ?? 5;
        }

        private void LFDelayChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BasicTeraData.Instance.WindowData.LFDelay = LFDelaySpinner?.Value ?? 150;
        }

        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ConfigScrollViewer.ScrollToVerticalOffset(ConfigScrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void PrivateChannelColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.PrivateChannelColor = (Color) e.NewValue;
        }

        private void EmotesColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.EmotesColor = (Color) e.NewValue;
        }

        private void TradingColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.TradingColor = (Color) e.NewValue;
        }

        private void SayColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.SayColor = (Color) e.NewValue;
        }

        private void RaidColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.RaidColor = (Color) e.NewValue;
        }

        private void GuildColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.GuildColor = (Color) e.NewValue;
        }

        private void GroupColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.GroupColor = (Color) e.NewValue;
        }

        private void GeneralColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.GeneralColor = (Color) e.NewValue;
        }

        private void AreaColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.AreaColor = (Color) e.NewValue;
        }

        private void AllianceColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.AllianceColor = (Color) e.NewValue;
        }

        private void WhisperColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            BasicTeraData.Instance.WindowData.WhisperColor = (Color) e.NewValue;
        }

        private void EnableRemoveTeraAltEnterHotkey(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.RemoveTeraAltEnterHotkey = true;
            KeyboardHook.Instance.Update();
        }

        private void DisableRemoveTeraAltEnterHotkey(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.RemoveTeraAltEnterHotkey = false;
            KeyboardHook.Instance.Update();
        }

        private void EnableChat(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.EnableChat = true;
            ChatSettingsVisible(true);
        }

        private void DisableChat(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.EnableChat = false;
            ChatSettingsVisible(false);
        }

        private void ChatSettingsVisible(bool show)
        {
            CopyInspect.Height = show ? double.NaN : 0;
            MuteSound.Height = show ? double.NaN : 0;
            PartyEvent.Height = show ? double.NaN : 0;
            ShowAfkIventsIngame.Height = show ? double.NaN : 0;
            WhisperColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            AllianceColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            AreaColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            GeneralColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            GroupColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            GuildColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            RaidColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            SayColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            TradingColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            EmotesColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
            PrivateChannelColor.Parent.SetValue(HeightProperty, show ? double.NaN : 0);
        }

        private void EnableCopyInspect(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.CopyInspect = true;
        }

        private void DisableCopyInspect(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.CopyInspect = false;
        }

        private void ClickUploadGlyphAction(object sender, RoutedEventArgs e)
        {
            if (_lastSend + TimeSpan.TicksPerSecond * 30 >= DateTime.Now.Ticks) { return; }
            if (string.IsNullOrEmpty(NetworkController.Instance.Glyphs.playerName)) { return; }
            if (NetworkController.Instance.EntityTracker.MeterUser.Level < 65) { return; }
            _lastSend = DateTime.Now.Ticks;
            var json = JsonConvert.SerializeObject(NetworkController.Instance.Glyphs,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore, TypeNameHandling = TypeNameHandling.None});
            Debug.WriteLine(json);
            Task.Run(() =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        //client.DefaultRequestHeaders.Add("X-Auth-Token", BasicTeraData.Instance.WindowData.TeraDpsToken);
                        //client.DefaultRequestHeaders.Add("X-User-Id", BasicTeraData.Instance.WindowData.TeraDpsUser);

                        client.Timeout = TimeSpan.FromSeconds(40);
                        var response = client.PostAsync("https://moongourd.com/shared/glyph_data.php", new StringContent(json, Encoding.UTF8, "application/json"));

                        var responseString = response.Result.Content.ReadAsStringAsync();
                        Debug.WriteLine(responseString.Result);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            });
        }

        private void EnableFormatPasteString(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.FormatPasteString = true;
        }

        private void DisableFormatPasteString(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.FormatPasteString = false;
        }


        private void ExcelCMADPSChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BasicTeraData.Instance.WindowData.ExcelCMADPSSeconds = ExcelCMADPSSpinner?.Value ?? 1;
        }

        private void DisablePartyEvent(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.DisablePartyEvent = true;
        }

        private void EnablePartyEvent(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.DisablePartyEvent = false;
        }

        private void EnableAfkIventsIngame(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.ShowAfkEventsIngame = true;
        }

        private void DisableAfkIventsIngame(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.ShowAfkEventsIngame = false;
        }

        private void ClickOpenChatBox(object sender, RoutedEventArgs e)
        {
            _mainWindow._chatbox = new Chatbox {Owner = _mainWindow};
            _mainWindow._chatbox.ShowWindow();
        }

        private void EnablePServerExp(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.PrivateServerExport = true;
            ServerURLTextbox.Parent.SetValue(HeightProperty, BasicTeraData.Instance.WindowData.PrivateServerExport ? double.NaN : 0);
        }

        private void DisablePServerExp(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.PrivateServerExport = false;
            ServerURLTextbox.Parent.SetValue(HeightProperty, BasicTeraData.Instance.WindowData.PrivateServerExport ? double.NaN : 0);
        }

        private void ServerURLChanged(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.PrivateDpsServers[0] = ServerURLTextbox.Text;
        }

        private void EnableMuteSound(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.MuteSound = true;
        }

        private void DisableMuteSound(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.MuteSound = false;
        }

        private void EnableShowSelfOnTop(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.MeterUserOnTop = true;
        }

        private void DisableShowSelfOnTop(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.MeterUserOnTop = false;
        }

        private void IdleRtoChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            BasicTeraData.Instance.WindowData.IdleResetTimeout = IdleRTOSpinner?.Value ?? 0;
            if (BasicTeraData.Instance.WindowData.IdleResetTimeout == 0) { DamageTracker.Instance.LastIdleStartTime = 0; }
        }

        private void TokenChanged(object sender, RoutedEventArgs e)
        {
            BasicTeraData.Instance.WindowData.TeraDpsToken = AuthTokenTextbox.Text;
        }

        private void AuthTokenTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { TokenChanged(this, new RoutedEventArgs()); }
        }

        private void ServerURLTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { ServerURLChanged(this, new RoutedEventArgs()); }
        }

        private void NoPaste_OnUnchecked(object sender, RoutedEventArgs e) { BasicTeraData.Instance.WindowData.NoPaste=false; }

        private void NoPaste_OnChecked(object sender, RoutedEventArgs e) { BasicTeraData.Instance.WindowData.NoPaste = true; }
    }
}