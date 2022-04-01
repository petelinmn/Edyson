using System;

namespace WorkerManager.Actors.Contract
{
    public class WorkerInfo
    {
        public Guid Id { get; set; }
        public string[] Args { get; set; }
        public WorkerStatus Status { get; set; }
        public string Tag { get; set; }
        public WorkerData Data { get; set; }
    }

    public class WorkerData
    {
        public int Counter { get; set; }
    }
}
