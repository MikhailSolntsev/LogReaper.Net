
using LogReaper.Net.Dto;

namespace LogReaper.Net;

internal class LogJournal
{
    private TextReader? fileReader = null;
    private OdinAssFileReader? readMachine = null;
    
    public void Open(string fileName)
    {
        fileReader = new StreamReader(File.OpenRead(fileName));
        readMachine = new OdinAssFileReader(fileReader);
    }

    public void OpenString(string input)
    {
        fileReader = new StringReader(input);
        readMachine = new OdinAssFileReader(fileReader);
    }

    public void Close()
    {
        fileReader?.Close();
    }

    public bool EOF()
    {
        if (readMachine is null) return true;
        return readMachine.EOF();
    }

    public LogRecord? ReadNext()
    {
        if (readMachine is null) return null;

        return ReadLogRecord(readMachine);
    }

    private static LogRecord? ReadLogRecord(OdinAssFileReader readMachine)
    {
        if (!readMachine.ReadBegin()) return null;

        var logRecord = new LogRecord();

        // 1		20200403000043
        logRecord.Datetime = long.Parse(readMachine.ReadValue());
        // 2		C
        logRecord.TransactionStatus = readMachine.ReadValue();
        // 3		{2438b1b2647b0,3d}
        logRecord.TransactionNumber = readMachine.ReadStructure();
        // 4		0
        logRecord.User = uint.Parse(readMachine.ReadValue());
        // 5		1
        logRecord.Computer = uint.Parse(readMachine.ReadValue());
        // 6		4
        logRecord.Application = uint.Parse(readMachine.ReadValue());
        // 7		2359592
        logRecord.Connection = uint.Parse(readMachine.ReadValue());
        //	8		6
        logRecord.EventId = uint.Parse(readMachine.ReadValue());
        //  9
        logRecord.Importance = readMachine.ReadValue();
        //  10
        logRecord.Comment = readMachine.ReadStructure();
        //  11
        logRecord.Metadata = uint.Parse(readMachine.ReadValue());
        //  12
        logRecord.Data = readMachine.ReadStructure();
        //  13
        logRecord.Representation = readMachine.ReadStructure();
        //  14
        logRecord.Server = uint.Parse(readMachine.ReadValue());
        //  15
        logRecord.Session = uint.Parse(readMachine.ReadValue());
        //  16
        readMachine.ReadValue();
        //  17
        readMachine.ReadValue();
        //  18
        readMachine.ReadValue();
        //  19
        var value = readMachine.ReadStructure();
        while (!String.IsNullOrEmpty(value))
        {
            value = readMachine.ReadStructure();
        }

        return logRecord;
    }
}
