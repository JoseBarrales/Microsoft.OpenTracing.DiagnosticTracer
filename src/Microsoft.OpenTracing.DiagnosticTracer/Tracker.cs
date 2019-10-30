using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenTracing.DiagnosticTracer
{
    /// <summary>
    /// Provides basic functionality to write custom events and logs for OpenTracing using System.Diagnostics
    /// </summary>
    public static class Tracker
    {
        /// <summary>
        /// Source name
        /// </summary>
        private static readonly string diagnosticListenerName = "TrackerEventListener";

        /// <summary>
        /// Diagnostic source instance
        /// </summary>
        private static readonly DiagnosticSource _source = new DiagnosticListener(diagnosticListenerName);

        /// <summary>
        /// Sends an error log event to observers
        /// </summary>
        /// <param name="tags">List of tags to be included in the trace</param>
        /// <param name="exception">Exception if any</param>
        /// <param name="errorEventName">Event name default's to 'Error'</param>
        public static void TrackError(IList<KeyValuePair<string, string>> tags, Exception exception, string errorEventName = "Error")
        {
            // Check if there is an observer listening
            if (_source.IsEnabled(errorEventName))
            {
                //write tags if any
                _source.Write(errorEventName, string.Join(Environment.NewLine, tags?.Select(x => $"{x.Key}={x.Value}")));
                _source.Write(errorEventName, exception);
            }
        }

        /// <summary>
        /// Send data to the event log
        /// </summary>
        /// <param name="data"></param>
        /// <param name="eventName"></param>
        public static void Track(string data, string eventName)
        {
            // Check if there is an observer listening
            if (_source.IsEnabled(eventName))
            {
                // write the event log with data
                _source.Write(eventName, data);
            }
        }

        /// <summary>
        /// Creates an IList<KeyValuePair<string, string>> adding the first tag element with specified key/value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<string, string>> WithTag(string key, string value)
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(key, value)
            };
        }

        /// <summary>
        /// Adds an element to the Tag list
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<string, string>> WithTag(this IList<KeyValuePair<string, string>> tags, string key, string value)
        {
            tags.Add(new KeyValuePair<string, string>(key, value));

            return tags;
        }

        /// <summary>
        /// Takes those tags and starts a new activity with custom OperationName
        /// </summary>
        /// <param name="tags">Tags list</param>
        /// <param name="operationName">Operation name to be presented</param>
        /// <returns></returns>
        public static Activity Start(this IList<KeyValuePair<string, string>> tags, string operationName)
        {
            // Check if there is an observer listening
            if (_source.IsEnabled(operationName))
            {
                // Creates a new activity with specified operation name
                var activity = new Activity(operationName);

                // pushing tags
                foreach (var tag in tags)
                {
                    activity.AddTag(tag.Key, $"{tag.Value}");
                }

                //finally just start the activity
                _source.StartActivity(activity, tags);

                //lets send the activity to the caller 'cause must be stoped at some point...
                return activity;
            }

            // if there is no one listening just return default
            return default;
        }

        /// <summary>
        /// Stops the activity to measure elapsed time
        /// </summary>
        /// <param name="activity"></param>
        public static void StopTrace(this Activity activity)
        {
            // we don't validate, just try to stop it
            _source.StopActivity(activity, new object());
        }
    }
}
