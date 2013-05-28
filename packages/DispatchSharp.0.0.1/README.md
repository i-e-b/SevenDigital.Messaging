DispatchSharp
=============

An experimental library to make multi-threaded dispatch code more testable.

Models a job dispatch pattern and provides both threaded and non threaded implementations.

Plans and requirements
----------------------

Limited-life job handlers: (IWorkerPool)
 * Deals with threading and names all threads [done]
 * Can add items and have them started [done]
 * can query number of items in flight [done]
 * can set a maximum number of jobs [done]
 * can set max to zero (as part of shutdown) [done]
 * can wait for all jobs to finish (as part of shutdown) [done]

Job managers: (IDispatch)
 * Given a ready/read/complete/cancel delegate (IWorkQueue), and a job handler [done]
 * sleeps until ready [done]
 * reads and sends to job handler [done]
 * completes if no exception [done]
 * cancels otherwise [done]
 * Can be stopped -- waits for job handler [done]
 * Can be started [done]

Work queue:
 * Encapsulate a ready/read/complete/cancel delegate [done]
 * can do persistent store and forward for waiting jobs.
 * can be queried for current length [done]

Safety valve
------------
An important feature when incoming jobs are spawning outgoing jobs.
One work dispatcher should be able to stop if another's work queue
exceeds a predefined length.