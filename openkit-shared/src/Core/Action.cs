﻿/***************************************************
 * (c) 2016-2017 Dynatrace LLC
 *
 * @author: Christian Schwarzbauer
 */
using Dynatrace.OpenKit.API;
using Dynatrace.OpenKit.Protocol;

namespace Dynatrace.OpenKit.Core {

    /// <summary>
    ///  Actual implementation of the IAction interface.
    /// </summary>
    public class Action : IAction {

        // Action ID, name and parent ID (default: null)
        private int id;
        private string name;
        private Action parentAction = null;

        // start/end time & sequence number
        private long startTime;
        private long endTime = -1;
        private int startSequenceNo;
        private int endSequenceNo = -1;

        // Beacon reference
        private Beacon beacon;

        // data structures for managing IAction hierarchies
        private SynchronizedQueue<IAction> openChildActions = new SynchronizedQueue<IAction>();
        private SynchronizedQueue<IAction> thisLevelActions = null;

        // *** constructors ***

        public Action(Beacon beacon, string name, SynchronizedQueue<IAction> parentActions) : this(beacon, name, null, parentActions) {
        }

        public Action(Beacon beacon, string name, Action parentAction, SynchronizedQueue<IAction> thisLevelActions) {
            this.beacon = beacon;
            this.parentAction = parentAction;

            this.startTime = TimeProvider.GetTimestamp();
            this.startSequenceNo = beacon.NextSequenceNumber;
            this.id = beacon.NextID;
            this.name = name;

            this.thisLevelActions = thisLevelActions;
            this.thisLevelActions.Put(this);
        }

        // *** IAction interface methods ***

        public IAction EnterAction(string actionName) {
            return new Action(beacon, actionName, this, openChildActions);
        }

        public IAction ReportEvent(string eventName) {
            beacon.ReportEvent(this, eventName);
            return this;
        }

        public IAction ReportValue(string valueName, string value) {
            beacon.ReportValue(this, valueName, value);
            return this;
        }

        public IAction ReportValue(string valueName, double value) {
            beacon.ReportValue(this, valueName, value);
            return this;
        }

        public IAction ReportValue(string valueName, int value) {
            beacon.ReportValue(this, valueName, value);
            return this;
        }

        public IAction ReportError(string errorName, int errorCode, string reason) {
            beacon.ReportError(this, errorName, errorCode, reason);
            return this;
        }

#if NET40 || NET35

        public IWebRequestTag TagWebRequest(System.Net.WebClient webClient) {
            return new WebRequestTagWebClient(beacon, this, webClient);
        }

#else

        public IWebRequestTag TagWebRequest(System.Net.Http.HttpClient httpClient) {
            return new WebRequestTagHttpClient(beacon, this, httpClient);
        }

#endif

        public IWebRequestTag TagWebRequest(string url) {
            return new WebRequestTagStringURL(beacon, this, url);
        }

        public IAction LeaveAction() {
            // check if leaveAction() was already called before by looking at endTime
            if (endTime != -1) {
                return parentAction;
            }

            // leave all open Child-Actions
            while (!openChildActions.IsEmpty()) {
                IAction action = openChildActions.Get();
                action.LeaveAction();
            }

            // set end time and end sequence number
            endTime = TimeProvider.GetTimestamp();
            endSequenceNo = beacon.NextSequenceNumber;

            // add Action to Beacon
            beacon.AddAction(this);

            // remove Action from the Actions on this level
            thisLevelActions.Remove(this);

            return parentAction;			// can be null if there's no parent Action!
        }

        // *** properties ***

        public int ID {
            get {
                return id;
            }
        }

        public string Name {
            get {
                return name;
            }
        }

        public int ParentID {
            get {
                return parentAction == null ? 0 : parentAction.ID;
            }
        }

        public long StartTime {
            get {
                return startTime;
            }
        }

        public long EndTime {
            get {
                return endTime;
            }
        }

        public int StartSequenceNo {
            get {
                return startSequenceNo;
            }
        }

        public int EndSequenceNo {
            get {
                return endSequenceNo;
            }
        }

    }

}