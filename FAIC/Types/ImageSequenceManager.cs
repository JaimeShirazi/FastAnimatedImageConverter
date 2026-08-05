using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FAIC.Types
{
    public class ImageSequenceManager
    {
        private readonly List<string> imagePaths;
        private readonly Dictionary<int, BitmapSource> frameCache = new();
        private readonly LinkedList<int> cacheOrder = new();
        private readonly Dictionary<int, LinkedListNode<int>> cacheNodes = new();
        private readonly int cacheCapacity;

        public int FrameCount => imagePaths.Count;
        public int CurrentFrameIndex { get; private set; }
        public BitmapSource CurrentFrame { get; private set; }
        public VideoFrameLayout FrameLayout { get; private set; }

        public ImageSequenceManager(List<string> imagePaths, int cacheCapacity = 3)
        {
            if (imagePaths == null)
                throw new ArgumentNullException(nameof(imagePaths));

            if (imagePaths.Count == 0)
                throw new ArgumentException("An image sequence must contain at least one frame.", nameof(imagePaths));
            if (imagePaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Image paths cannot be null or empty.", nameof(imagePaths));

            this.imagePaths = imagePaths;
            this.cacheCapacity = Math.Max(1, cacheCapacity);
            CurrentFrame = GetFrame(0);
            CurrentFrameIndex = 0;
            FrameLayout = VideoFrameLayout.FullFrame(CurrentFrame.PixelWidth, CurrentFrame.PixelHeight);
        }
        public bool Step(int frameDelta)
        {
            int target = Math.Clamp(CurrentFrameIndex + frameDelta, 0, FrameCount - 1);
            return SetFrame(target);
        }
        public bool SetFrame(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= FrameCount)
                throw new ArgumentOutOfRangeException(nameof(frameIndex));
            if (frameIndex == CurrentFrameIndex && CurrentFrame != null)
                return false;

            BitmapSource frame = GetFrame(frameIndex);

            if (frame.PixelWidth != FrameLayout.CodedWidth || frame.PixelHeight != FrameLayout.CodedHeight)
            {
                throw new InvalidDataException(
                    $"Image sequence frame {frameIndex} is {frame.PixelWidth}x{frame.PixelHeight}; " +
                    $"expected {FrameLayout.CodedWidth}x{FrameLayout.CodedHeight}.");
            }

            CurrentFrame = frame;
            CurrentFrameIndex = frameIndex;
            return true;
        }
        private BitmapSource GetFrame(int frameIndex)
        {
            if (frameCache.TryGetValue(frameIndex, out BitmapSource? cached))
            {
                LinkedListNode<int> foundCacheNode = cacheNodes[frameIndex];
                cacheOrder.Remove(foundCacheNode);
                cacheOrder.AddFirst(foundCacheNode);
                return cached;
            }

            //Load frame
            string path = imagePaths![frameIndex];
            BitmapSource loaded;
            using (FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapDecoder decoder = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);
                BitmapSource frame = decoder.Frames[0];
                if (!frame.IsFrozen && frame.CanFreeze)
                {
                    loaded = (BitmapSource)frame.GetAsFrozen();
                }
                else
                {
                    loaded = frame;
                }
            }

            frameCache.Add(frameIndex, loaded);
            LinkedListNode<int> node = cacheOrder.AddFirst(frameIndex);
            cacheNodes.Add(frameIndex, node);

            while (frameCache.Count > cacheCapacity)
            {
                int removeIndex = cacheOrder.Last!.Value;
                cacheOrder.RemoveLast();
                cacheNodes.Remove(removeIndex);
                frameCache.Remove(removeIndex);
            }

            return loaded;
        }
        public void Dispose()
        {
            frameCache.Clear();
            cacheOrder.Clear();
            cacheNodes.Clear();
        }
    }
}
