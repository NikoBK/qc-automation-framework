using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text.Json;
using PMCLIB;
using System.Threading.Tasks;

namespace AutomationFramework
{
    /// <summary>
    /// Handles instances of <cref="Vial"> (digital representation) redirected to it by the <cref="MES">.
    /// </summary>
    public class LiquidHandler : Station
    {
        //------------------------------------------- Variables -------------------------------------------
        private readonly HttpClient _client;
        private readonly string _url;

        private SystemCommands _systemCommand = new SystemCommands();
        private XBotCommands _xbotCommand = new XBotCommands();

        //------------------------------------------ Constructor ------------------------------------------
        public LiquidHandler(string Ip)
        {
            Vector2 entrance = new Vector2(100, 100);
            Vector2 exit = new Vector2(200, 200);
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("opentrons-version", "2");
            string ot2_ip = Ip;
            _url = $"http://{ot2_ip}:31950";
            Start("OT2 Liquid Handler", entrance, exit);
            Task.Run(Main);
        }

        //---------------------------------------- Private Methods ---------------------------------------- 
        // I need to add methods for moving the shuttle, which can only be done when the OT2 has been integrated onto the table

        private async Task<string?> UploadProtocol(string protocolPath)
        {
            logger.Log($"Trying to upload protocol with path {protocolPath}");
            if (!File.Exists(protocolPath))
            {
                logger.Log($"Protocol on {protocolPath} not found!");
                return null;
            }
            logger.Log($"Found protocol on {protocolPath}. Continuing with upload");

            using var form = new MultipartFormDataContent();
            
            // Adding protocol file
            using var fileStream = File.OpenRead(protocolPath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            form.Add(fileContent, "files", Path.GetFileName(protocolPath));
            logger.Log($"Added protocol file from path {protocolPath}");

            // Adding custom labware
            string labwarePath = "C:\\Users\\Nicolai\\qc-automation-framework\\AutomationFramework\\labware\\";
            string[] labwareFiles = Directory.GetFiles(labwarePath, "*.json", SearchOption.TopDirectoryOnly);
            foreach ( string labwareFile in labwareFiles)
            {
                var labwareStream = File.OpenRead(labwareFile);
                var labwareContent = new StreamContent(labwareStream);
                labwareContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                form.Add(labwareContent, "files", Path.GetFileName(labwareFile));
            }
            logger.Log($"Added all custom labware from labware path: {labwarePath}");

            HttpResponseMessage response = await _client.PostAsync($"{_url}/protocols", form);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                string protocolId = doc.RootElement.GetProperty("data").GetProperty("id").GetString() ?? "";

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    logger.Log($"Protocol uploaded successfully with protocol id: {protocolId}\n");
                }
                else
                {
                    logger.Log($"Protocol already uploaded, using existing protocol with id: {protocolId}\n");
                }
                return protocolId;
            }
            else
            {
                logger.Log($"Protocol upload failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }

        private async Task<string?> CreateRun(string protocolId)
        {
            var payload = new
            {
                data = new
                {
                    protocolId = protocolId
                }
            };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_url}/runs", content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                string runId = doc.RootElement.GetProperty("data").GetProperty("id").GetString() ?? "";
                logger.Log($"Run created successfully with id: {runId}\n");
                return runId;
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                logger.Log($"Failed to create run. Status: {response.StatusCode}, Error: {error}");
                return null;
            }
        }

        private async Task<bool> PlayRun(string runId)
        {
            var payload = new
            {
                data = new
                {
                    actionType = "play"
                }
            };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"{_url}/runs/{runId}/actions";
            HttpResponseMessage response = await _client.PostAsync(url, content);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                logger.Log($"Successfully played run with run id: {runId}\n");
                return true;
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                logger.Log($"Failed to play run with run id: {runId}:\nError: {response.StatusCode}, {error}\n");
                return false;
            }
        }

        private async Task<string?> GetRunStatus(string runId)
        {
            var url = $"{_url}/runs/{runId}";
            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(content);
                string status = doc.RootElement.GetProperty("data").GetProperty("status").GetString();
                return status;
            }
            else
            {
                logger.Log($"Failed to get run status. Status code: {response.StatusCode}");
                return null;
            }
        }

        private async Task Run()
        {
            // Implement queue function
            logger.Log($"Running with Rack ID {rack.rackId}");
            string path = rack.protocolPath;
            string protocolId = await UploadProtocol(path);
            string runId = await CreateRun(protocolId);
            PlayRun(runId);
            string runStatus = await GetRunStatus(runId);


            string? lastStatus = null;
            while (runStatus != "finishing")
            {
                runStatus = await GetRunStatus(runId);
                if (lastStatus != runStatus)
                {
                    logger.Log($"Run {runId} status: {runStatus}");
                }
                lastStatus = runStatus;
            }
            while (runStatus == "finishing")
            {
                runStatus = await GetRunStatus(runId);
            }
            logger.Log($"Run {runId} finished");

            ChangeState(MachineState.IDLE);
        }

        //---------------------------------------  Public Methods ----------------------------------------
        public void RecieveRack(VialRack newRack)
        {
            // also add logic here to fit a queue if machine is not idle
            rack = newRack;
            ChangeState(MachineState.RUNNING);
        }

        //---------------------------------------- Main Function -----------------------------------------
        private async Task Main()
        {
            MachineState? lastState = null;

            while (true)
            {
                if (State == MachineState.STOPPED)
                {
                    logger.Log($"[{Name}] has stopped.");
                    break;
                }

                switch (State)
                {
                    case MachineState.IDLE:
                        break;

                    case MachineState.RUNNING:
                        await Run();
                        break;

                    case MachineState.FAULT:
                        logger.Log($"[{Name}] in error state.");
                        break;
                }
            }
        }
    }

    public class DeCapper : Station
    {
        //------------------------------------------- Variables -------------------------------------------
        private Dictionary<string, Vector2> vialPos = new Dictionary<string, Vector2>
            {
                {"A1", new Vector2(0.083f,0.773f)},
                {"A2", new Vector2(0.103f,0.773f)},
                {"A3", new Vector2(0.123f,0.773f)},
                {"A4", new Vector2(0.143f,0.773f)},
                {"B1", new Vector2(0.083f,0.793f)},
                {"B2", new Vector2(0.103f,0.793f)},
                {"B3", new Vector2(0.123f,0.793f)},
                {"B4", new Vector2(0.143f,0.793f)},
                {"C1", new Vector2(0.083f,0.813f)},
                {"C2", new Vector2(0.103f,0.813f)},
                {"C3", new Vector2(0.123f,0.813f)},
                {"C4", new Vector2(0.143f,0.813f)},
                {"D1", new Vector2(0.083f,0.833f)},
                {"D2", new Vector2(0.103f,0.833f)},
                {"D3", new Vector2(0.123f,0.833f)},
                {"D4", new Vector2(0.143f,0.833f)},
                {"E1", new Vector2(0.083f,0.853f)},
                {"E2", new Vector2(0.103f,0.853f)},
                {"E3", new Vector2(0.123f,0.853f)},
                {"E4", new Vector2(0.143f,0.853f)}
            };

        private SystemCommands _systemCommand = new SystemCommands();
        private XBotCommands _xbotCommand = new XBotCommands();

        //------------------------------------------ Constructor ------------------------------------------
        public DeCapper()
        {
            Vector2 entrance = new Vector2(0.300f, 0.900f);
            Vector2 exit = new Vector2(0.300f, 0.78f);
            Start("(De)Capper", entrance, exit);

            Task.Run(Main);
        }
        //---------------------------------------- Private Methods ---------------------------------------- 
        private void Cap()
        {
            TcpClient cli = new TcpClient(Constants.CapHandlerAddr, Constants.CapHandlerPort);
            NetworkStream str = cli.GetStream();
            string msg = "cap";
            byte[] dat = Encoding.UTF8.GetBytes(msg);
            str.Write(dat, 0, dat.Length);
            cli.Close();
            Thread.Sleep(2000);
        }
        private void Decap()
        {
            TcpClient cli = new TcpClient(Constants.CapHandlerAddr, Constants.CapHandlerPort);
            NetworkStream str = cli.GetStream();
            string msg = "decap";
            byte[] dat = Encoding.UTF8.GetBytes(msg);
            str.Write(dat, 0, dat.Length);
            cli.Close();
            Thread.Sleep(2000);
        }

        private async Task OpenGripper()
        {
            TcpClient cli = new TcpClient(Constants.CapHandlerAddr, Constants.CapHandlerPort);
            NetworkStream str = cli.GetStream();
            string msg = "open gripper";
            byte[] dat = Encoding.UTF8.GetBytes(msg);
            str.Write(dat, 0, dat.Length);
            cli.Close();
            await Task.Delay(500);
        }

        private async Task CloseGripper()
        {
            TcpClient cli = new TcpClient(Constants.CapHandlerAddr, Constants.CapHandlerPort);
            NetworkStream str = cli.GetStream();
            string msg = "close gripper";
            byte[] dat = Encoding.UTF8.GetBytes(msg);
            str.Write(dat, 0, dat.Length);
            cli.Close();
            await Task.Delay(500);
        }

        private async Task MoveActuator(int height_mm)
        {
            TcpClient cli = new TcpClient(Constants.FestoAddr, Constants.FestoPort);
            NetworkStream str = cli.GetStream();
            string msg = $"{height_mm}";
            byte[] dat = Encoding.UTF8.GetBytes(msg);
            str.Write(dat, 0, dat.Length);
            cli.Close();
            await Task.Delay(1000);
        }

        private async Task MoveToVial(string vial)
        {
            if (vialPos.TryGetValue(vial, out Vector2 position))
            {
                MotionRtn motionRtn = _xbotCommand.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.YTHENX, position.X, position.Y, 0, 0.5, 10);
                double time = motionRtn.TravelTimeSecs * 1000;
                await Task.Delay((int)time);
            }
            else
            {
                logger.Log($"Failed to move rack to {vial}");
            }


        }
        private async Task Run()
        {
            await MoveActuator(32);
            await OpenGripper();
            _xbotCommand.LinearMotionSI(0, rack.rackId, POSITIONMODE.RELATIVE, LINEARPATHTYPE.YTHENX, -0.120f, 0, 0, 0.5, 10);
            // loop through recipe (what vials need decapping)
            foreach (var vial in rack.rack)
            {
                // Move rack to vial pos
                await MoveToVial(vial.Key);
                // lower actuator
                await MoveActuator(0);
                // engage gripper
                await CloseGripper();
                // raise actuator
                await MoveActuator(32); // val needs to be measured    
                // decap
                await Task.Delay(1000);
                // lower actuator
                await MoveActuator(0);
                // release gripper
                await OpenGripper();
                // raise actuator
                await MoveActuator(32); // val needs to be measured 
                // remove rack
                // collect cap
            }


            _xbotCommand.LinearMotionSI(0, rack.rackId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.YTHENX, Exit.X, Exit.Y, 0, 0.5, 10);
            ChangeState(MachineState.IDLE);
        }

        //---------------------------------------  Public Methods ----------------------------------------
        public void RecieveRack(VialRack newRack)
        {
            rack = newRack;
            ChangeState(MachineState.RUNNING);
        }

        //---------------------------------------- Main Function -----------------------------------------
        private async Task Main()
        {
            MachineState? lastState = null;

            while (true)
            {
                if (State == MachineState.STOPPED)
                {
                    Console.WriteLine($"[{Name}] has stopped.");
                    break;
                }

                // Only react when state changes, or on loop
                if (State != lastState)
                {
                    Console.WriteLine($"[{Name}] state changed to: {State}");
                    lastState = State;
                }

                switch (State)
                {
                    case MachineState.IDLE:
                        break;

                    case MachineState.RUNNING:
                        await Run();
                        break;

                    case MachineState.FAULT:
                        Console.WriteLine($"[{Name}] in error state.");
                        break;
                }
            }
        }
    }

    public class Decapper : Station
    {
        //TODO
    }

    public class VialMixer : Station
    {
        //---------------------------------------  Variables ----------------------------------------
        private SystemCommands _systemCommand = new SystemCommands();
        private XBotCommands _xbotCommand = new XBotCommands();
        //TODO

        //---------------------------------------  Constructor ----------------------------------------
        public VialMixer()
        {
            Vector2 entrance = new Vector2(0.660f, 0.420f);
            Vector2 exit = new Vector2(0.540f, 0.420f);

            Start("VialMixer", entrance, exit);

            Task.Run(Main);
        }

        //---------------------------------------  Private Methods ----------------------------------------
        private void mix(int xbot_id, int method, double shakeMag, double duration)
        {
            double motionTime = 0;
            MotionRtn motionRtn;

            if (method == 0)
            {
                _xbotCommand.MotionBufferControl(xbot_id, MOTIONBUFFEROPTIONS.BLOCKBUFFER);
                while (motionTime < duration)
                {
                    motionRtn = _xbotCommand.LinearMotionSI(0, xbot_id, POSITIONMODE.RELATIVE, LINEARPATHTYPE.DIRECT, shakeMag / 1000, shakeMag / 1000, 0, 5, 10);
                    motionTime += motionRtn.TravelTimeSecs;
                    motionRtn = _xbotCommand.LinearMotionSI(0, xbot_id, POSITIONMODE.RELATIVE, LINEARPATHTYPE.DIRECT, -(shakeMag / 1000), -(shakeMag / 1000), 0, 5, 10);
                    motionTime += motionRtn.TravelTimeSecs;
                }
                _xbotCommand.MotionBufferControl(xbot_id, MOTIONBUFFEROPTIONS.RELEASEBUFFER);
            }
        }

        private async Task Run()
        {
            MotionRtn motionRtn;
            int id = rack.rackId;
            logger.Log("Moving rack to center of mover station");
            motionRtn = _xbotCommand.LinearMotionSI(0, id, POSITIONMODE.RELATIVE, LINEARPATHTYPE.YTHENX, -0.060f, 0.180f, 0, 0.5, 10);
            double time = motionRtn.TravelTimeSecs * 1000;
            await Task.Delay((int)time);

            logger.Log("Mixing rack");
            mix(id, 0, 3, 8);

            logger.Log("Moving rack to exit, and setting state to IDLE");
            _xbotCommand.LinearMotionSI(0, id, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.XTHENY, Exit.X, Exit.Y, 0, 0.5, 10);
            ChangeState(MachineState.IDLE);
        }

        //---------------------------------------  Public Methods ----------------------------------------
        public void RecieveRack(VialRack newRack)
        {
            rack = newRack;
            ChangeState(MachineState.RUNNING);
        }

        //---------------------------------------  Main Functions ----------------------------------------
        private async Task Main()
        {
            MachineState? lastState = null;

            while (true)
            {
                if (State == MachineState.STOPPED)
                {
                    logger.Log($"[{Name}] has stopped.");
                    break;
                }

                switch (State)
                {
                    case MachineState.IDLE:
                        break;

                    case MachineState.RUNNING:
                        await Run();
                        break;

                    case MachineState.FAULT:
                        logger.Log($"[{Name}] in error state.");
                        break;
                }
            }
        }
    }
}
