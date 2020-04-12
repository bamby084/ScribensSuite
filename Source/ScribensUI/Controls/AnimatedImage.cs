using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace PluginScribens.UI.Controls
{
    public class AnimatedImage : Image
    {
        public static readonly DependencyProperty GifSourceProperty = DependencyProperty.Register(
            "GifSource",
            typeof(ImageSource),
            typeof(AnimatedImage),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                OnGifSourceChanged,
                null));

        public static readonly DependencyProperty RepeatProperty = DependencyProperty.Register(
            "Repeat",
            typeof(RepeatBehavior),
            typeof(AnimatedImage),
            new FrameworkPropertyMetadata(RepeatBehavior.Forever, OnRepeatChanged));

        private static readonly DependencyProperty CurrentFrameIndexProperty = DependencyProperty.Register(
            "CurrentFrameIndex",
            typeof(int),
            typeof(AnimatedImage),
            new PropertyMetadata(0, OnCurrentFrameIndexChanged));


        public ImageSource GifSource
        {
            get => (ImageSource)GetValue(GifSourceProperty);
            set => SetValue(GifSourceProperty, value);
        }

        public RepeatBehavior Repeat
        {
            get => (RepeatBehavior)GetValue(RepeatProperty);
            set => SetValue(RepeatProperty, value);
        }

        private int CurrentFrameIndex
        {
            get => (int)GetValue(CurrentFrameIndexProperty);
            set => SetValue(CurrentFrameIndexProperty, value);
        }

        public void StartAnimation()
        {
            var bitmapFrame = this.GifSource as BitmapFrame;
            if (bitmapFrame == null)
                return;

            int frameCount = bitmapFrame.Decoder.Frames.Count;
            if (frameCount == 1)
                return;

            var firstFrame = bitmapFrame.Decoder.Frames[0];
            var metaData = firstFrame.Metadata as BitmapMetadata;
            var frameDuration = (ushort)metaData.GetQuery("/grctlext/Delay");
            var animation = new Int32Animation(0, frameCount - 1,
                new Duration(TimeSpan.FromMilliseconds(frameDuration * 10 * frameCount)))
            {
                RepeatBehavior = Repeat
            };

            BeginAnimation(CurrentFrameIndexProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        public void StopAnimation()
        {
            BeginAnimation(CurrentFrameIndexProperty, null);
        }

        private static void OnCurrentFrameIndexChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AnimatedImage image = (AnimatedImage)dependencyObject;
            image.SetFrame((int)e.NewValue);
        }

        private static void OnGifSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AnimatedImage image = (AnimatedImage)dependencyObject;
            image.Source = image.GifSource;
            image.StartAnimation();
        }

        private static void OnRepeatChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AnimatedImage image = (AnimatedImage)dependencyObject;
            image.StartAnimation();
        }

        private void SetFrame(int frameIndex)
        {
            this.Source = ((BitmapFrame)GifSource).Decoder.Frames[frameIndex];
        }
    }
}
