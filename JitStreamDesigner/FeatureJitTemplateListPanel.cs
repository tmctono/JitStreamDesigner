﻿// Copyright (c) Manabu Tonosaki All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graphics.Canvas.Text;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tono;
using Tono.Gui;
using Tono.Gui.Uwp;
using Tono.Jit;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using static Tono.Gui.Uwp.CastUtil;

namespace JitStreamDesigner
{
    /// <summary>
    /// Feature : Jit Template List Panel Control
    /// </summary>
    /// <remarks>
    /// [EventCatch(TokenID = TOKEN.TemplateSelectionChanged)]  EventTokenTemplateChangedTrigger 
    /// </remarks>
    [FeatureDescription(En = "Template Control Panel", Jp = "テンプレート一覧パネル")]
    public class FeatureJitTemplateListPanel : FeatureSimulatorBase
    {
        public static class TOKEN
        {
            public const string TemplateSelectionChanged = "TemplateSelectionChanged";
        }

        private PartsActiveTemplate BarParts = null;

        /// <summary>
        /// target list view UWP control
        /// </summary>
        public ListView TargetListView { get; set; }

        /// <summary>
        /// initialize feature
        /// </summary>
        public override void OnInitialInstance()
        {
            // UWP control preparation
            if (TargetListView == null)
            {
                Kill(new NullReferenceException("FeatureJitTemplateListPanel must have FeatureJitTemplateListPanel={Bind:****}"));
                return;
            }

            TargetListView.ItemsSource = Hot.TemplateList;
            TargetListView.SelectionChanged += TargetListView_SelectionChanged;

            // Add default template chip
            DelayUtil.Start(TimeSpan.FromMilliseconds(200), () =>
            {
                AddTemplateChip("@Default", Colors.Yellow, "Free GUI space");
            });

            // Draw preparation
            Pane.Target = Pane["LogPanel"]; // to get priority draw layer
            BarParts = new PartsActiveTemplate
            {
            };
            Parts.Add(Pane.Target, BarParts, LAYER.ActiveTemplate);
        }

        /// <summary>
        /// List selection change UWP Control event handling
        /// To change UWP event to token trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TargetListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tar = e.AddedItems.LastOrDefault() as TemplateTipModel;
            if (tar != Hot.ActiveTemplate)
            {
                Token.AddNew(new EventTokenTemplateChangedTrigger
                {
                    TargetTemplate = tar,
                    TokenID = TOKEN.TemplateSelectionChanged,
                    Sender = this,
                    Remarks = "Select changed List View to change active template",
                });
            }
        }


        /// <summary>
        /// Template change request event
        /// </summary>
        /// <param name="token"></param>
        [EventCatch(TokenID = TOKEN.TemplateSelectionChanged)]
        public void TemplateSelectionChanged(EventTokenTemplateChangedTrigger token)
        {
            if (ReferenceEquals(Hot.ActiveTemplate, token.TargetTemplate)) return;

            Hot.ActiveTemplate = token.TargetTemplate;
            BarParts.Text = Hot.ActiveTemplate?.Name;
            BarParts.BackgroundColor = Hot.ActiveTemplate.AccentColor;
            BarParts.Remarks = Hot.ActiveTemplate?.Remarks;
            Redraw();

            if (object.ReferenceEquals(TargetListView.SelectedItem, token.TargetTemplate) == false)
            {
                TargetListView.SelectedItem = token.TargetTemplate; // for if not called TargetListView_SelectionChanged
            }
        }

        /// <summary>
        /// Add template chip and initialize Jac objects
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="accentColor"></param>
        /// <param name="remarks"></param>
        private void AddTemplateChip(string templateName, Color accentColor, string remarks)
        {
            var te = new TemplateTipModel
            {
                Template = new JitTemplate
                {
                    Name = templateName,
                },
                AccentColor = accentColor,
                Remarks = remarks,
                Jac = new JacInterpreter(),
            };
            var jac = $@"
                TheStage = new Stage
                    Name = '{templateName}'
                    AccentColor = {accentColor.ToString()}
                    Remarks = '{remarks}'
            ";
            te.Jac.Exec(jac);
            te.Jac["Gui"] = Hot.TheBroker;

            Hot.TemplateList.Add(te);
            TargetListView.SelectedItem = te;   // auto select the new item
        }

        [EventCatch]
        public async Task AddTemplate(EventTokenButton token)
        {
            try
            {
                // Show Dialog
                var td = new TemplateDialog
                {
                    AccentColor = (new ColorUtil.HSV((float)MathUtil.Rand() * 360, 1.0f, 1.0f)).ToColor(),
                };
                Hot.KeybordShortcutDisabledFlags["AddTemplate"] = true;
                var res = await td.ShowAsync();

                if (res == ContentDialogResult.Primary)
                {
                    AddTemplateChip(td.TemplateName, td.AccentColor, td.TemplateRemarks);
                    LOG.AddMes(LLV.INF, new LogAccessor.Image { Key = "Lump16" }, "FeatureJitTemplatePanel-CannotUndo");
                }
            }
            finally
            {
                Hot.KeybordShortcutDisabledFlags["AddTemplate"] = false;
            }
        }
    }

    /// <summary>
    /// Token type for template selection change
    /// </summary>
    public class EventTokenTemplateChangedTrigger : EventTokenTrigger
    {
        public TemplateTipModel TargetTemplate { get; set; }
    }

    /// <summary>
    /// Active Template name and Yellow Bar
    /// </summary>
    public class PartsActiveTemplate : PartsBase<ScreenX, ScreenY>
    {
        public string Text { get; set; }
        public Color BackgroundColor { get; set; }
        public string Remarks { get; set; }

        public override void Draw(DrawProperty dp)
        {
            if (string.IsNullOrEmpty(Text))
            {
                return;
            }
            // Yellow bar
            var r = dp.PaneRect.Clone();
            r.LT = ScreenPos.From(r.L, 0);
            r.RB = ScreenPos.From(r.R, 2);
            dp.Graphics.FillRectangle(_(r), BackgroundColor);

            // Active Template name (Back ground)
            var tf = new CanvasTextFormat
            {
                FontFamily = "Coureir New",
                FontSize = 11.0f,
                FontWeight = FontWeights.Normal,
                WordWrapping = CanvasWordWrapping.NoWrap,
            };
            r.RB = r.LT + GraphicUtil.MeasureString(dp.Canvas, Text, tf) + ScreenSize.From(10, 10);
            dp.Graphics.FillRectangle(_(r), BackgroundColor);

            // Active Template name (Text)
            dp.Graphics.TextAntialiasing = CanvasTextAntialiasing.ClearType;
            dp.Graphics.DrawText(Text, 4, 2, ColorUtil.GetNegativeColor(BackgroundColor), tf);
        }
    }
}