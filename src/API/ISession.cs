﻿/***************************************************
 * (c) 2016-2017 Dynatrace LLC
 *
 * @author: Christian Schwarzbauer
 */
namespace Dynatrace.OpenKit.API {

    /// <summary>
    ///  This interface provides functionality to create Actions in a Session.
    /// </summary>
    public interface ISession {

        /// <summary>
        ///  Enters an Action with a specified name in this Session.
        /// </summary>
        /// <param name="actionName">name of the Action</param>
        /// <returns>Action instance to work with</returns>
        IAction EnterAction(string actionName);

        /// <summary>
        ///  Reports a crash with a specified error name, crash reason and a stacktrace.
        /// </summary>
        /// <param name="errorName">name of the error leading to the crash (e.g. Exception class)</param>
        /// <param name="reason">reason or description of that error</param>
        /// <param name="stacktrace">stacktrace leading to that crash</param>
        void ReportCrash(string errorName, string reason, string stacktrace);

        /// <summary>
        ///  Ends this Session and marks it as finished for sending.
        /// </summary>
        void End();

    }

}
