﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassicUO.Game.Gumps.Controls;
using ClassicUO.Game.Gumps.Controls.InGame;
using ClassicUO.Input;
using ClassicUO.Renderer;
using ClassicUO.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.Gumps.UIGumps
{
    class SkillGumpAdvanced : Gump
    {
        private ScrollArea _scrollArea;
        private Texture2D _blackTexture;
        private Texture2D _line;
        private List<SkillListEntry> _skillListEntries;
        private bool _updateSkillsNeeded;

        public SkillGumpAdvanced() : base(0, 0)
        {
            _skillListEntries = new List<SkillListEntry>();
            _line = new Texture2D(Service.Get<SpriteBatch3D>().GraphicsDevice, 1, 1);
            _line.SetData(new[] { Color.White });


            X = 100;
            Y = 100;
            CanMove = true;
            AcceptMouseInput = false;

            AddChildren(new GameBorder(0, 0, 320, 330));

            AddChildren(new GumpPicTiled(4, 6, 320 - 8, 330 - 12, 0x0A40) { IsTransparent =  true});
            AddChildren(new GumpPicTiled(4, 6, 320 - 8, 330 - 12, 0x0A40) { IsTransparent = true });

            _scrollArea = new ScrollArea(20, 60, 295, 250, true) { AcceptMouseInput = true };
            AddChildren(_scrollArea);
            AddChildren(new Label("Skill", true, 1153) { X = 30, Y = 25 });
            AddChildren(new Label("Real", true, 1153) { X = 165, Y = 25 });
            AddChildren(new Label("Base", true, 1153) { X = 195, Y = 25 });
            AddChildren(new Label("Cap", true, 1153) { X = 250, Y = 25 });


            World.Player.SkillsChanged += OnSkillChanged;
        }

        protected override void OnInitialize()
        {
            _scrollArea.Clear();

            foreach (var entry in _skillListEntries)
            {
                entry.Clear();
                entry.Dispose();
            }
            _skillListEntries.Clear();

            foreach (Skill skill in World.Player.Skills)
            {
                Label skillName = new Label(skill.Name, true, 1153) { Font = 3 }; //3
                Label skillValueBase = new Label(skill.Base.ToString(), true, 1153) { Font = 3 };
                Label skillValue = new Label(skill.Value.ToString(), true, 1153) { Font = 3 };
                Label skillCap = new Label(skill.Cap.ToString(), true, 1153) { Font = 3 };
                _skillListEntries.Add(new SkillListEntry(skillName, skillValueBase, skillValue, skillCap, skill));
            }

            for (int i = 0; i < _skillListEntries.Count; i++)
            {
                _scrollArea.AddChildren(_skillListEntries[i]);
            }
        }

        public override bool Draw(SpriteBatchUI spriteBatch, Vector3 position, Vector3? hue = null)
        {
            spriteBatch.Draw2D(_line, new Rectangle((int)position.X + 30, (int)position.Y + 50, 260, 1), RenderExtentions.GetHueVector(0, false, .5f, false));
            return base.Draw(spriteBatch, position, hue);

        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (_updateSkillsNeeded)
            {
                OnInitialize();
                _updateSkillsNeeded = false;
            }

        }

        public override void Dispose()
        {
            World.Player.SkillsChanged -= OnSkillChanged;
            base.Dispose();
        }
       

        private void OnSkillChanged(object sender, EventArgs args)
        {
            _updateSkillsNeeded = true;
        }
    }

    public class SkillListEntry : GumpControl
    {
        public readonly Label SkillName;
        public readonly Label SkillValueBase;
        public readonly Label SkillValue;
        public readonly Label SkillCap;
        public readonly Skill Skill;


        private readonly SpriteTexture[] _textures = new SpriteTexture[3]
        {
            IO.Resources.Gumps.GetGumpTexture(0x983),
            IO.Resources.Gumps.GetGumpTexture(0x985),
            IO.Resources.Gumps.GetGumpTexture(0x82C),
        };

        public SkillListEntry(Label skillname, Label skillvaluebase, Label skillvalue, Label skillcap, Skill skill)
        {
            Height = 20;

            SkillName = skillname;
            SkillValueBase = skillvaluebase;
            SkillValue = skillvalue;
            SkillCap = skillcap;
            Skill = skill;


            SkillName.X = 10;
            AddChildren(SkillName);
            //======================
            SkillValueBase.X = 150;
            AddChildren(SkillValueBase);
            //======================
            SkillValue.X = 180;
            AddChildren(SkillValue);
            //======================
            SkillCap.X = 230;
            AddChildren(SkillCap);
        }

        public override void Update(double totalMS, double frameMS)
        {
            _textures.ForEach(s => s.Ticks = (long)totalMS);
            base.Update(totalMS, frameMS);
        }

        public override bool Draw(SpriteBatchUI spriteBatch, Vector3 position, Vector3? hue = null)
        {
            base.Draw(spriteBatch, position, hue);

            spriteBatch.Draw2D(_textures[(int) Skill.Lock], new Vector3(position.X + 210, position.Y + 5, position.Z),
                Vector3.Zero);

            return true;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left && x >= 210 && x <= 210 + _textures[(int)Skill.Lock].Width && y >= 0 && y <= _textures[(int) Skill.Lock].Height)
            {
                switch (Skill.Lock)
                {
                    case SkillLock.Up:
                        Skill.Lock = SkillLock.Down;

                        break;
                    case SkillLock.Down:
                        Skill.Lock = SkillLock.Locked;
                        break;
                    case SkillLock.Locked:
                        Skill.Lock = SkillLock.Up;
                        break;
                }
            }
        }
    }

}

