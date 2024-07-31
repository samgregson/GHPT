using FuzzySharp;
using FuzzySharp.Extractor;
using GHPT.Prompts;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GHPT.Utils
{
    public static class GraphUtil
    {
        private static readonly Dictionary<int, IGH_DocumentObject> CreatedComponents = new();


        public static void InstantiateComponent(GH_Document doc, Prompts.Components component, System.Drawing.PointF pivot)
        {
            try
            {
                string name = component.Name;
                IGH_ObjectProxy myProxy = GetObject(name);
                if (myProxy is null)
                    return;

                Guid myId = myProxy.Guid;

                if (CreatedComponents.ContainsKey(component.Id))
                {
                    CreatedComponents.Remove(component.Id);
                }

                var emit = Instances.ComponentServer.EmitObject(myId);
                CreatedComponents.Add(component.Id, emit);

                doc.AddObject(emit, false);
                emit.Attributes.Pivot = pivot;
                SetValue(component, emit);
            }
            catch
            {
            }
        }

        public static void ConnectComponent(GH_Document doc, ConnectionPairing pairing)
        {
            CreatedComponents.TryGetValue(pairing.From.Id, out IGH_DocumentObject componentFrom);
            CreatedComponents.TryGetValue(pairing.To.Id, out IGH_DocumentObject componentTo);

            IGH_Param fromParam = GetParam(componentFrom, pairing.From, false);
            IGH_Param toParam = GetParam(componentTo, pairing.To, true);

            if (fromParam is null || toParam is null)
            {
                return;
            }

            toParam.AddSource(fromParam);
            toParam.CollectData();
            toParam.ComputeData();
        }

        private static IGH_Param GetParam(IGH_DocumentObject docObj, Connection connection, bool isInput)
        {
            var resultParam = docObj switch
            {
                IGH_Param param => param,
                IGH_Component component => GetComponentParam(component, connection, isInput),
                _ => null
            };

            return resultParam;
        }

        private static IGH_Param GetComponentParam(IGH_Component component, Connection connection, bool isInput)
        {
            IList<IGH_Param> _params = (isInput ? component.Params.Input : component.Params.Output)?.ToArray();

            if (_params is null || _params.Count == 0)
                return null;

            if (_params.Count() <= 1)
                return _params.First();

            // Linq Alternative to below
            // _params.First(p => p.Name.ToLowerInvariant() == connection.ParameterName.ToLowerInvariant());
            foreach (var _param in _params)
            {
                if (_param.Name.ToLowerInvariant() == connection.ParameterName.ToLowerInvariant())
                {
                    return _param;
                }
            }

            ExtractedResult<string> fuzzyResult = Process.ExtractOne(connection.ParameterName, _params.Select(_p => _p.Name));
            if (fuzzyResult.Score >= 50)
            {
                return _params[fuzzyResult.Index];
            }

            return null;
        }

        private static IGH_ObjectProxy GetObject(string name)
        {
            IGH_ObjectProxy[] results = Array.Empty<IGH_ObjectProxy>();
            double[] resultWeights = new double[] { 0 };
            var exactMatch = Instances.ComponentServer.FindObjectByName( name, false, false);
            if (exactMatch != null) { return exactMatch as IGH_ObjectProxy; ; }

            Instances.ComponentServer.FindObjects(new string[] { name }, 10, ref results, ref resultWeights);

            // get culmulative sum of probabilities
            var weightSum = resultWeights.Sum();
            var probabilities = resultWeights.Select(w => w / weightSum);
            var cumSum = probabilities.Select((p, i) => probabilities.Take(i).Sum());

            // get results until 50% cumSum probability
            results = results.TakeWhile((r, i) => cumSum.ElementAt(i) <= 0.5).ToArray();

            var myProxies = results.Where(ghpo => ghpo.Kind == GH_ObjectType.CompiledObject);

            var _components = myProxies.Where(p => p.Type.BaseType.Name == "GH_Component");
            var _params = myProxies.Where(p => p.Type.BaseType.Name.Contains("Param"));

            // Prefer Components to Params
            var myProxy = myProxies.First();
            if (_components.Any())
            {
                var component = _components.FirstOrDefault();
                myProxy = component as IGH_ObjectProxy;
            }
            else if (_params.Any())
            {
                var param = _params.FirstOrDefault();
                myProxy = param as IGH_ObjectProxy;
            }

            return myProxy;
        }

        private static void SetValue(Prompts.Components component, IGH_DocumentObject ghProxy)
        {
            string lowerCaseName = component.Name.ToLowerInvariant();

            bool result = ghProxy switch
            {
                GH_NumberSlider slider => SetNumberSliderData(component, slider),
                GH_Panel panel => SetPanelData(component, panel),
                Param_Point point => SetPointData(component, point),
                _ => false
            };

        }

        private static bool SetPointData(Prompts.Components component, Param_Point point)
        {
            try
            {
                if (string.IsNullOrEmpty(component.Value))
                    return false;

                string[] pointValues = component.Value.Replace("{", "").Replace("}", "").Split(',');
                double[] pointDoubles = pointValues.Select(p => double.Parse(p)).ToArray();

                point.SetPersistentData(new Rhino.Geometry.Point3d(pointDoubles[0], pointDoubles[1], pointDoubles[2]));
            }
            catch
            {
                point.SetPersistentData(new Rhino.Geometry.Point3d(0, 0, 0));
            }
            finally
            {
                point.CollectData();
                point.ComputeData();
            }

            return true;
        }

        private static bool SetPanelData(Prompts.Components component, GH_Panel panel)
        {
            panel.SetUserText(component.Value);
            return true;
        }

        private static bool SetNumberSliderData(Prompts.Components component, GH_NumberSlider slider)
        {
            string value = component.Value;
            if (string.IsNullOrEmpty(value)) return false;
            slider.SetInitCode(value);

            return true;
        }

    }
}
