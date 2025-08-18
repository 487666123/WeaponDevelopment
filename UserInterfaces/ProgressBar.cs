using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.BasicElements;

namespace WeaponDevelopment.UserInterfaces;

[XmlElementMapping("WD-ProgressBar")]
public class ProgressBar : UIView
{
    public event EventHandler<ValueChangedEventArgs<float>> ProgressChanged;

    public virtual float Progress
    {
        get;
        set
        {
            // 如果值未变化，直接返回
            if (field == value) return;

            // 捕获旧值
            float oldProgress = field;

            // 更新为新值
            field = value;

            // 触发事件，传递旧值和新值
            HandleProgressChanged(oldProgress, value);
        }
    }

    protected virtual void HandleProgressChanged(float oldValue, float newValue)
    {
        ProgressChanged?.Invoke(this, new(oldValue, newValue));
    }

    private readonly RectangleRender _bar = new();

    public ProgressBar()
    {
        _bar.BackgroundColor = Color.Black * 0.25f;
    }

    protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        base.Draw(gameTime, spriteBatch);

        var position = Bounds.Position;
        var size = Bounds.Size;
        size *= new Size(Progress, 1f);

        _bar.BorderRadius = new Vector4(Math.Min(size.Width, size.Height) / 2);
        _bar.Draw(position, size, false, SilkyUI.TransformMatrix);
    }
}