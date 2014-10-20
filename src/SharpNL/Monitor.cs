using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace SharpNL {
    /// <summary>
    /// Represents a SharpNL task that can be monitored or canceled. This class cannot be inherited.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Monitor {
        private readonly CancellationTokenSource cancelSource;

        private Task task;

        public Monitor() {
            cancelSource = new CancellationTokenSource();           
        }

        #region + Events .

        /// <summary>
        /// Occurs when the task completes.
        /// </summary>
        public event EventHandler Complete;

        /// <summary>
        /// Occurs when the object that is running the task sends an informational message.
        /// </summary>
        public event EventHandler<MonitorMessageEventArgs> Message;

        /// <summary>
        /// Occurs when the object that is running the task sends a warning message.
        /// </summary>
        public event EventHandler<MonitorMessageEventArgs> Warning;

        /// <summary>
        /// Occurs when an exception is throw during the task execution.
        /// </summary>
        public event EventHandler<MonitorExceptionEventArgs> Exception; 
        #endregion

        #region + Properties .

        #region . IsCanceled .
        /// <summary>
        /// Gets whether this <see cref="Monitor"/> instance has completed execution due to being canceled.
        /// </summary>
        /// <value><c>true</c> if the task has completed due to being canceled; otherwise, <c>false</c>.</value>
        public bool IsCanceled { get; private set; }
        #endregion

        #region . IsRunning .
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning { get; internal set; }
        #endregion

        #region . TotalMessages .
        /// <summary>
        /// Gets the number of messages in the current execution.
        /// </summary>
        /// <value>The number of messages in the current execution.</value>
        [Description("The number of messages in the current execution.")]
        public int TotalMessages { get; private set; }
        #endregion

        #region . TotalWarnings .
        /// <summary>
        /// Gets the number of warnings in the current execution.
        /// </summary>
        /// <value>The number of warnings in the current execution.</value>
        [Description("The number of warnings in the current execution.")]
        public int TotalWarnings { get; private set; }
        #endregion

        #region . TotalExceptions .
        /// <summary>
        /// Gets the number of exceptions in the current execution.
        /// </summary>
        /// <value>The number of exceptions in the current execution.</value>
        [Description("The number of exceptions in the current execution.")]
        public int TotalExceptions { get; private set; }
        #endregion

        #region . Token .
        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        /// <value>The cancellation token.</value>
        internal CancellationToken Token {
            get {
                if (cancelSource != null)
                    return cancelSource.Token;

                return default(CancellationToken);
            }
        }

        #endregion

        #endregion

        #region . Cancel .
        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel() {
            cancelSource.Cancel();
            try {
                task.Wait(Token);    
            } catch (OperationCanceledException) {
                IsCanceled = true;
                IsRunning = false;
            }           
        }
        #endregion

        #region . Execute .
        /// <summary>
        /// Executes the specified task action.
        /// </summary>
        /// <param name="taskAction">The task action.</param>
        internal void Execute(Action<CancellationToken> taskAction) {
            IsRunning = true;

            task = Task.Factory.StartNew(() => {
                try {
                    taskAction(cancelSource.Token);
                } catch (OperationCanceledException) {
                    IsCanceled = true;
                } catch (AggregateException ae) {
                    ae.Handle(ex => {
                        if (ex is TaskCanceledException) {
                            IsCanceled = true;
                            return true;
                        }
                        OnException(ex);
                        return true;
                    });
                } catch (Exception ex) {
                    OnException(ex);
                } finally {                   
                    IsRunning = false;
                    task = null;

                    if (Complete != null)
                        Complete(this, EventArgs.Empty);

                }
            }, cancelSource.Token);
        }

        #endregion

        #region . OnException .
        /// <summary>
        /// Process the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        internal void OnException(Exception exception) {
            TotalExceptions++;
            if (Exception != null)
                Exception(this, new MonitorExceptionEventArgs(exception));
        }
        #endregion

        #region . OnMessage .
        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        internal void OnMessage(string message) {
            TotalMessages++;
            if (Message != null)
                Message(this, new MonitorMessageEventArgs(message));
        }
        #endregion

        #region . OnWarning .
        /// <summary>
        /// Processes the warning message.
        /// </summary>
        /// <param name="message">The warning message.</param>
        internal void OnWarning(string message) {
            TotalWarnings++;
            if (Warning != null)
                Warning(this, new MonitorMessageEventArgs(message));
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Resets this counters in this instance.
        /// </summary>
        public void Reset() {
            TotalExceptions = 0;
            TotalMessages = 0;
            TotalWarnings = 0;
        }
        #endregion

        #region . Wait .
        /// <summary>
        /// Waits for the <see cref="Monitor"/> to complete execution.
        /// </summary>
        public void Wait() {
            if (task != null)
                task.Wait(Token);
        }
        #endregion


    }
}