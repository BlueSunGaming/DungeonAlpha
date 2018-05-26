using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class RPGEventHandler : MonoBehaviour
    {
        private List<EventContainer> _currentEvents;
        public int CurEvents = 0;

        void Update()
        {
            CurEvents = _currentEvents.Count;
        }

        void Start()
        {
            _currentEvents = new List<EventContainer>();
        }

        public EventContainer RunRoutine(IEnumerator routine)
        {
            var newEvent = new EventContainer();
            _currentEvents.Add(newEvent);
            StartCoroutine(HandleRoutine(newEvent, routine));
            return newEvent;
        }

        public bool EventIsRunning(string eventId)
        {
            return _currentEvents.FirstOrDefault(e => e.Id == eventId) != null;
        }

        IEnumerator HandleRoutine(EventContainer eventToRun, IEnumerator routine)
        {
            eventToRun.Status = EventStatus.InProgress;

            //handle event
            yield return StartCoroutine(routine);

            eventToRun.Status = EventStatus.Complete;
            GetObject.RPGCamera.cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;


            yield return new WaitForSeconds(0.2f);
            _currentEvents.Remove(eventToRun);
        }   

        public EventContainer RunEvent(string eventId)
        {
            //todo: check if we can run event first
            //var eventToRun = Rm_RPGHandler.Instance.Nodes.EventNodeChains.FirstOrDefault(e => e.CurrentNode.ID == eventId);
            var eventTree = Rm_RPGHandler.Instance.Nodes.EventNodeBank.NodeTrees.FirstOrDefault(n => n.Nodes.FirstOrDefault(x => x.ID == eventId) != null);
            if (eventTree == null) eventTree = Rm_RPGHandler.Instance.Nodes.EventNodeBank.NodeTrees.FirstOrDefault(n => n.ID == eventId);
            NodeChain nodeChain;
            if(eventTree != null)
            {
                var evt = eventTree.Nodes.FirstOrDefault(n => n.ID == eventId) ?? eventTree.Nodes.FirstOrDefault(n => n is EventStartNode);
                if (evt == null) return new EventContainer { Status = EventStatus.Failed };
                nodeChain = new NodeChain(eventTree, evt);
            }
            else
            {
                return new EventContainer {Status = EventStatus.Failed};
            }

            var newEvent = new EventContainer(nodeChain);
            _currentEvents.Add(newEvent);
            StartCoroutine(HandleEvent(newEvent));
            return newEvent;
        }

        public EventContainer RunNodeChain(NodeChain nodeChainToRun)
        {
            var newEvent = new EventContainer(nodeChainToRun);
            _currentEvents.Add(newEvent);
            StartCoroutine(HandleEvent(newEvent));
            return newEvent;
        }

        IEnumerator HandleEvent(EventContainer eventToRun)
        {
            yield return null;
            eventToRun.Status = EventStatus.InProgress;

            //handle event

            var nodeChain = eventToRun.EventChain;
            while (!nodeChain.Done)
            {
                nodeChain.Evaluate();
                yield return null;
            }

            eventToRun.Status = EventStatus.Complete;
            GetObject.RPGCamera.cameraMode = Rm_RPGHandler.Instance.DefaultSettings.DefaultCameraMode;

            yield return new WaitForSeconds(0.2f);
            _currentEvents.Remove(eventToRun);
        }
    }

    public class EventContainer
    {
        public string Id;
        public NodeChain EventChain;
        public EventStatus Status;
        public bool Done
        {
            get { return Status == EventStatus.Complete || Status == EventStatus.Failed; }
        }

        public EventContainer(NodeChain eventChain)
        {
            Id = Guid.NewGuid().ToString();
            EventChain = eventChain;
            Status = EventStatus.NotStarted;
        }

        public EventContainer()
        {
            Id = Guid.NewGuid().ToString();
            Status = EventStatus.NotStarted;
        }
    }

    public enum EventStatus
    {
        NotStarted,
        InProgress,
        Complete,
        Failed
    }
}