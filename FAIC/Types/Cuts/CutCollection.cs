using System;
using System.Collections.Generic;

namespace FAIC.Types.Cuts
{
    /// <summary>
    /// The authoritative collection used directly by CutsControl.
    /// All mutations must occur on the Windows Forms UI thread.
    /// </summary>
    public class CutCollection
    {
        public event Action OnNewSelectedValues = delegate { };
        public event Action OnNewSelectionOrSize = delegate { };

        private readonly List<Cut> _cuts = new() { new Cut(0m, 1m) };
        private int _selected;
        private int _lastHovering = -1;

        // Lets CutsControl detect re-entrant or otherwise incompatible source
        // edits without maintaining a second segment collection.
        internal long Revision { get; private set; }

        public int Selected => _selected;
        public int Total => _cuts.Count;
        public decimal GetTotalLengthRatio()
        {
            decimal sum = 0;
            for (int i = 0; i < _cuts.Count; i++)
            {
                sum += _cuts[i].End - _cuts[i].Start;
            }
            return sum;
        }

        public Cut this[int index]
        {
            get
            {
                ValidateIndex(index);
                return _cuts[index];
            }
            set
            {
                ValidateIndex(index);
                ValidateCut(value, nameof(value));

                if (_cuts[index].Equals(value))
                {
                    return;
                }

                _cuts[index] = value;
                NotifyValuesChanged();
            }
        }

        /// <summary>
        /// Atomically applies a group of value-only updates and raises one
        /// value notification. Structural operations intentionally live in
        /// AddAfter, AddSplit, RemoveAt, and Reset instead.
        /// </summary>
        internal bool ApplyBatch(IReadOnlyDictionary<int, Cut> updates)
        {
            ArgumentNullException.ThrowIfNull(updates);

            foreach (KeyValuePair<int, Cut> update in updates)
            {
                ValidateIndex(update.Key);
                ValidateCut(update.Value, nameof(updates));
            }

            bool changed = false;
            foreach (KeyValuePair<int, Cut> update in updates)
            {
                if (_cuts[update.Key].Equals(update.Value))
                {
                    continue;
                }

                _cuts[update.Key] = update.Value;
                changed = true;
            }

            if (changed)
            {
                NotifyValuesChanged();
            }

            return changed;
        }

        public bool ShouldSelectedSplitNotAdd() => _cuts[Selected].End >= 1m - Program.NormalizedMinimumCutLength;

        public void AddButton(decimal normalizedTime)
        {
            if (ShouldSelectedSplitNotAdd())
            {
                AddSplit(Selected, normalizedTime);
            }
            else
            {
                AddAfter(Selected);
            }
        }

        public void RemoveButton() => RemoveAt(Selected);

        public int AddAfter(int index)
        {
            ValidateIndex(index);

            if (_cuts[index].End >= 1m)
            {
                return AddSplit(index, 0m);
            }

            int insertedIndex = index + 1;
            Cut newCut = new Cut(_cuts[index].End, 1m)
            {
                NormalizedCrop = _cuts[index].NormalizedCrop
            };
            _cuts.Insert(insertedIndex, newCut);
            NotifyStructureChanged();
            return insertedIndex;
        }

        public int AddSplit(int index, decimal normalizedTime)
        {
            ValidateIndex(index);

            Cut existingCut = _cuts[index];
            decimal cutPosition = (existingCut.Start + existingCut.End) / 2m;

            if (normalizedTime > existingCut.Start &&
                normalizedTime < existingCut.End)
            {
                cutPosition = normalizedTime;
            }

            var secondHalf = new Cut(cutPosition, existingCut.End)
            {
                NormalizedCrop = existingCut.NormalizedCrop
            };

            existingCut.End = cutPosition;
            _cuts[index] = existingCut;
            _cuts.Insert(index + 1, secondHalf);
            NotifyStructureChanged();
            return index + 1;
        }

        public void RemoveAt(int index)
        {
            ValidateIndex(index);
            _cuts.RemoveAt(index);
            NotifyStructureChanged();
        }

        public int GetFirstCutAtTime(decimal time, decimal videoLength = 1m)
        {
            decimal normalizedPosition = videoLength > 0m ? time / videoLength : 0m;
            for (int i = 0; i < _cuts.Count; i++)
            {
                if (_cuts[i].Overlaps(normalizedPosition))
                {
                    return i;
                }
            }

            return -1;
        }

        public void OnNewSelectedValue(int newValue)
        {
            ValidateIndex(newValue);
            if (newValue == _selected) return;

            _selected = newValue;
            OnNewSelectionOrSize.Invoke();
        }

        public void OnPlayheadMoved(decimal newNormalizedTime)
        {
            int newHovering = GetFirstCutAtTime(newNormalizedTime);
            if (newHovering == _lastHovering)
            {
                return;
            }

            _lastHovering = newHovering;
            if (newHovering >= 0 && newHovering != _selected)
            {
                _selected = newHovering;
                OnNewSelectionOrSize.Invoke();
            }
        }

        public void Reset()
        {
            _cuts.Clear();
            NotifyStructureChanged();
        }

        private void NotifyValuesChanged()
        {
            unchecked
            {
                Revision++;
            }

            OnNewSelectedValues.Invoke();
        }

        private void NotifyStructureChanged()
        {
            if (_cuts.Count == 0)
            {
                _cuts.Add(new Cut(0m, 1m));
            }

            _selected = Math.Clamp(_selected, 0, _cuts.Count - 1);
            _lastHovering = -1;

            unchecked
            {
                Revision++;
            }

            OnNewSelectionOrSize.Invoke();
        }

        private void ValidateIndex(int index)
        {
            if ((uint)index >= (uint)_cuts.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "The cut index is outside the collection.");
            }
        }

        private static void ValidateCut(Cut cut, string parameterName)
        {
            if (!cut.IsValid())
            {
                throw new ArgumentException("A cut must have valid ordered endpoints and a non-empty crop.", parameterName);
            }
        }
    }
}