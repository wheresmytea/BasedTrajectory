using System;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Sequences;
using UnityEngine.Sequences.Timeline;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace UnityEditor.Sequences
{
    /// <summary>
    /// Selection utilities for MasterSequence Toolbox.
    /// It goes beyond the global selection by filtering certain types of data
    /// useful for MasterSequence Toolbox.
    /// </summary>
    internal static class SelectionUtility
    {
        /// <summary>
        /// Triggered when a new Sequence is selected.
        /// </summary>
        public static event Action sequenceSelectionChanged;

        /// <summary>
        /// Triggered when a new PlayableDirector is selected.
        /// </summary>
        public static event Action playableDirectorChanged;

        /// <summary>
        /// Store the active selection from the Sequences window.
        /// </summary>
        static TimelineSequence m_ActiveSequence;

        /// <summary>
        /// Store the active selection of PlayableDirector.
        /// </summary>
        static PlayableDirector m_ActivePlayableDirector;

        /// <summary>
        /// Active selection from the Sequences window.
        /// </summary>
        public static TimelineSequence activeSequenceSelection => m_ActiveSequence;

        /// <summary>
        /// Active PlayableDirector selected.
        /// </summary>
        public static PlayableDirector activePlayableDirector => m_ActivePlayableDirector;

        /// <summary>
        /// Method marked as InitializeOnLoad to allow this class to listen for Editor events.
        /// </summary>
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            Selection.selectionChanged += OnEditorSelectionChanged;
        }

        // Callback reacting to new selection from the Editor global selection.
        // That way, we can make sure our selection system follows the Editor selection.
        static void OnEditorSelectionChanged()
        {
            if (Selection.activeGameObject == null)
            {
                // Sends an event if activePlayableDirector is null.
                // It likely got destroyed.
                if (activePlayableDirector == null)
                    SelectAndNotifyPlayableDirector(null);

                return;
            }

            if (!IsPlayableDirector(Selection.activeGameObject, out PlayableDirector director))
                return;

            var sequence = SequenceUtility.GetSequenceFromTimeline(director.playableAsset as TimelineAsset);
            if (sequence != null)
            {
                // TimelineSequence has been selected.
                SelectSequence(sequence);
            }
            else
            {
                // A Sequence Asset playable director has been selected.
                TrySetTimelineInContextOf(director);
            }
        }

        /// <summary>
        /// Set a Timeline in the context of the given TimelineSequence.
        /// </summary>
        /// <param name="clipDirector"></param>
        /// <param name="owner"></param>
        static void TrySetTimelineInContextOf(TimelineSequence owner)
        {
            TimelineUtility.breadcrumb.Clear();
            TimelineUtility.breadcrumb.BuildAndAppend(owner);
            TimelineUtility.RefreshBreadcrumb();
        }

        /// <summary>
        /// Try to set a given PlayableDirector in Timeline with its context.
        /// Requirements:
        /// - Must be a Sequence Asset.
        /// - Must exist in a TimelineSequence hierarchy.
        /// </summary>
        /// <param name="director"></param>
        static void TrySetTimelineInContextOf(PlayableDirector director)
        {
            var sequenceAsset = PrefabUtility.GetNearestPrefabInstanceRoot(director.gameObject);
            if (sequenceAsset == null ||
                !SequenceAssetUtility.IsSequenceAsset(sequenceAsset))
            {
                SelectPlayableDirector(director);
                return;
            }

            var parentClip = HierarchyUtility.GetFirstParentOfType<SequenceFilter>(sequenceAsset);

            // Check if object is under a TimelineSequence gameObject.
            // If it is, we can move on to the next check.
            if (parentClip == null || !parentClip.TryGetComponent(out PlayableDirector parentPlayableDirector))
            {
                SelectPlayableDirector(director);
                return;
            }

            var sequence = SequenceUtility.GetSequenceFromTimeline(parentPlayableDirector.playableAsset as TimelineAsset);
            if (sequence != null)
            {
                var breadcrumb = TimelineUtility.breadcrumb;
                breadcrumb.Clear();

                var sequenceAssetClip = SequenceAssetUtility.GetClipFromInstance(sequenceAsset, parentPlayableDirector);
                if (sequenceAssetClip == null)
                {
                    SelectPlayableDirector(director);
                    return;
                }

                breadcrumb.Append(director, sequenceAssetClip);
                breadcrumb.BuildAndAppend(sequence);
                TimelineUtility.RefreshBreadcrumb(breadcrumb);
            }
            SelectPlayableDirector(parentPlayableDirector);
        }

        /// <summary>
        /// Returns true if the given GameObject has a PlayableDirector.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="director"></param>
        /// <returns></returns>
        static bool IsPlayableDirector(GameObject selection, out PlayableDirector director)
        {
            director = selection.GetComponent<PlayableDirector>();
            return director != null;
        }

        /// <summary>
        /// Set the active Sequence to the given one.
        /// Will prevent multiple assignment of the same Sequence unless <paramref name="force"/> is set to true.
        /// </summary>
        /// <param name="sequence">Sequence to assign as active selection.</param>
        static void SelectSequence(TimelineSequence sequence)
        {
            m_ActiveSequence = sequence;
            TrySetTimelineInContextOf(sequence);
            SelectPlayableDirector(sequence);
            sequenceSelectionChanged?.Invoke();
        }

        /// <summary>
        /// Try to select the gameObject linked to the given TimelineSequence.
        /// </summary>
        /// <param name="sequence"></param>
        public static void TrySelectSequence(TimelineSequence sequence)
        {
            var director = sequence.timeline.FindDirector();
            if (director != null)
                Selection.activeGameObject = director.gameObject;
        }

        /// <summary>
        /// Set the activePlayableDirector to the reference given as parameter.
        /// Will fire playableDirectorChanged event once it's done.
        /// </summary>
        /// <param name="director"></param>
        static void SelectPlayableDirector(PlayableDirector director)
        {
            if (director == m_ActivePlayableDirector)
                return;

            m_ActivePlayableDirector = director;
            playableDirectorChanged?.Invoke();
        }

        /// <summary>
        /// Set the activePlayableDirector to the reference given as parameter
        /// even if it has been already selected.
        /// Will fire playableDirectorChanged event once it's done.
        /// </summary>
        /// <param name="director"></param>
        static void SelectAndNotifyPlayableDirector(PlayableDirector director)
        {
            m_ActivePlayableDirector = director;
            playableDirectorChanged?.Invoke();
        }

        /// <summary>
        /// Select the corresponding PlayableDirector for the given TimelineSequence.
        /// Works only if it exists in a loaded scene.
        /// </summary>
        /// <param name="sequence"></param>
        static void SelectPlayableDirector(TimelineSequence sequence)
        {
            if (sequence == null)
            {
                SelectPlayableDirector((PlayableDirector)null);
                return;
            }

            if (sequence.timeline == null)
                throw new NullReferenceException("timelineSequence.timeline");

            var director = sequence.timeline.FindDirector();

            if (director)
                SelectPlayableDirector(director);
        }

        /// <summary>
        /// Open the given Timeline in TimelineEditor.
        /// </summary>
        /// <param name="timeline"></param>
        public static void SelectTimeline(TimelineAsset timeline)
        {
            SetSelectionForTimelinePlayableDirector(timeline);
        }

        /// <summary>
        /// Try select an existing Playable Director that references the given timeline in parameter.
        /// If it finds none, it selects the timeline masterSequence.
        /// </summary>
        /// <param name="timeline"></param>
        /// <returns>True when a Playable Director is found. Otherwise, returns false.</returns>
        static bool SetSelectionForTimelinePlayableDirector(TimelineAsset timeline)
        {
            if (timeline == null)
                throw new System.NullReferenceException("timeline");

            // Hack, needs to talk to devs-timeline
            IReadOnlyCollection<PlayableDirector> playables = ObjectsCache.FindObjectsFromScenes<PlayableDirector>();
            foreach (var p in playables)
            {
                if (p.playableAsset == timeline)
                {
                    Selection.activeObject = p.gameObject;
                    TimelineEditor.Refresh(RefreshReason.WindowNeedsRedraw);
                    return true;
                }
            }
            Selection.activeObject = timeline;
            return false;
        }

        /// <summary>
        /// Select the given GameObject in the Hierarchy or Project view.
        /// </summary>
        /// <param name="sequenceAsset"></param>
        internal static void SetSelection(GameObject sequenceAsset)
        {
            Selection.activeObject = sequenceAsset;
        }
    }
}
