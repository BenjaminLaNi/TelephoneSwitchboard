using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PortsController : MonoBehaviour
{
    public Dictionary<PortController, Action> incomingCalls = new Dictionary<PortController, Action>();
    public List<Call> currentCalls = new List<Call>();
    public List<RegisteredConnection> registeredConnections = new List<RegisteredConnection>();
    public ScoreHandler scoreHandler;

    List<PortController> ports;
    public Dictionary<string, List<CableJackController>> cables;
    public GameObject cablesParent;
    public List<Connection> requiredConnections = new List<Connection>();
    public string requiredOperatorId = "";
    public Action requiredOperatorCallback = new Action(() => { });
    // Start is called before the first frame update
    void Awake()
    {
        ports = new List<PortController>(this.GetComponentsInChildren<PortController>());
        cables = new Dictionary<string, List<CableJackController>>();
        foreach (CableJackController c in cablesParent.GetComponentsInChildren<CableJackController>())
        {
            if (!cables.ContainsKey(c.gameObject.transform.parent.gameObject.name))
            {
                cables.Add(c.gameObject.transform.parent.gameObject.name, new List<CableJackController>());
            }
            cables[c.gameObject.transform.parent.gameObject.name].Add(c);
        }

        foreach (PortController port in ports)
        {
            port.OccupiedCallback = (isOccupied) =>
            {
                if (!isOccupied) { return; }
                if (incomingCalls.ContainsKey(port) && isOccupied && port.IsOperatorCable)
                {
                    incomingCalls[port]();
                    incomingCalls.Remove(port);
                    return;
                }
                var cable = cables.Values.ToList().FindLast((c) => c.Contains(port.cableConnected));
                if (cable[0].connection == null || cable[1].connection == null || cable[0].connection.IsOperatorCable || cable[1].connection.IsOperatorCable) { return; }
                if (!(requiredConnections.FindAll((c) => c.includesPort(cable[0].connection)).Count > 0 || requiredConnections.FindAll((c) => c.includesPort(cable[1].connection)).Count > 0))
                {
                    RegisterConnection(new Connection(cable[0].connection, cable[1].connection), ConnectionType.Unnecessary);
                    return;
                }
                foreach (Connection connection in requiredConnections.FindAll((c) => c.includesPort(cable[0].connection) || c.includesPort(cable[1].connection)))
                {
                    if (connection.includesPort(cable[0].connection) && connection.includesPort(cable[0].connection))
                    {
                        RegisterConnection(connection, ConnectionType.Correct);
                        return;
                    }
                    RegisterConnection(connection, ConnectionType.Wrong);
                }
            };
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequireConnection(PortController first, PortController second)
    {
        requiredConnections.Add(new Connection(first, second));
    }

    public void RequireConnection(string firstId, string secondId)
    {
        RequireConnection(ports.Find(p => p.PortName == firstId), ports.Find(p => p.PortName == secondId));
    }

    public void RequireOperator(string id, Action callback)
    {
        PortController _p = ports.Where((p) => p.PortName == id).FirstOrDefault();
        incomingCalls.Add(_p, callback);
        _p.ToggleLight(true);
        //_p.BlinkLight();
    }

    public void StartCall(PortController caller, PortController callee)
    {
        Call call = new Call()
        {
            caller = caller,
            callee = callee,
            callerName = "",
            calleeName = ""
        };
        currentCalls.Add(call);
        callee.BlinkLight();

        IEnumerator endCall(Call call, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            currentCalls.Remove(call);
            call.caller.ToggleLight(false);
            call.callee.ToggleLight(false);
        }

        StartCoroutine(endCall(call, new System.Random().Next(10, 15)));
    }

    public void StartCall(Connection connection)
    {
        Call call = new Call()
        {
            caller = connection.caller,
            callee = connection.callee,
            callerName = "",
            calleeName = ""
        };
        currentCalls.Add(call);
        connection.callee.BlinkLight();

        IEnumerator endCall(Call call, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            currentCalls.Remove(call);
            call.caller.ToggleLight(false);
            call.callee.ToggleLight(false);
        }

        StartCoroutine(endCall(call, new System.Random().Next(10, 15)));
    }

    public void RegisterConnection(Connection connection, ConnectionType type)
    {
        registeredConnections.Add(new RegisteredConnection(connection, type));
        if (type == ConnectionType.Correct)
        {
            StartCall(connection.caller, connection.callee);
            requiredConnections.Remove(connection);
        }
        scoreHandler.GetGrade();
    }
}

public interface ICall
{
    public PortController caller { get; set; }
    public string callerName { get; set; }
    public PortController callee { get; set; }
    public string calleeName { get; set; }
}

public class Call : ICall
{
    public PortController caller { get; set; }
    public string callerName { get; set; }
    public PortController callee { get; set; }
    public string calleeName { get; set; }
}

public class Connection : ICall
{
    public PortController caller { get; set; }
    public string callerName { get; set; }
    public PortController callee { get; set; }
    public string calleeName { get; set; }
    public DateTime startTime { get; private set; }

    public Connection(PortController caller, PortController callee)
    {
        this.caller = caller;
        this.callee = callee;
        this.calleeName = ""; this.callerName = "";
        this.startTime = DateTime.Now;
    }

    public bool includesPort(PortController port)
    {
        return caller == port || callee == port;
    }
}

public class RegisteredConnection
{
    public Connection connection { get; set; }
    public ConnectionType type { get; set; }
    public TimeSpan timeElapsed { get; private set; }

    public RegisteredConnection(Connection connection, ConnectionType type)
    {
        this.connection = connection;
        this.type = type;
        timeElapsed = DateTime.Now.Subtract(connection.startTime);
    }
}

public enum ConnectionType
{
    Correct,
    Wrong,
    Unnecessary
}