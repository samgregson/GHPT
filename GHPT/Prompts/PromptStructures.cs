using System.Collections.Generic;
using System.Linq;

namespace GHPT.Prompts
{

    public struct PromptData
    {
        public string Advice { get; set; }
        public IEnumerable<Component> Components { get; set; }
        public IEnumerable<ConnectionPairing> Connections { get; set; }

        public void ComputeTiers()
        {
            List<Component> components = this.Components.ToList();
            if (components == null)
                return;

            for (int i = 0; i < components.Count(); i++)
            {
                Component component = components[i];
                int tier = FindParentsRecursive(component);
                component.Tier = tier;

                components[i] = component;
            }
            this.Components = components;
        }

        public int FindParentsRecursive(Component child, int depth = 0)
        {
            try
            {
                List<ConnectionPairing> pairings = Connections.Where(c => c.To.Id == child.Id).ToList();
                List<int> depths = new();
                foreach (ConnectionPairing pairing in pairings)
                {
                    if (pairing.IsValid())
                    {
                        Component parent = Components.FirstOrDefault(a => pairing.From.Id == a.Id);
                        int maxDepth = FindParentsRecursive(parent, depth + 1);
                        depths.Add(maxDepth);
                    }
                }

                if (depths.Count == 0) return depth;
                else if (depths.Count == 1) return depths[0];
                else return depths.Max();
            }
            catch
            {
                return depth;
            }
        }
    }

    public struct Component
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string? Value { get; set; }
        public int Tier { get; set; }
    }

    public struct ConnectionPairing
    {
        public Connection To { get; set; }
        public Connection From { get; set; }

        public bool IsValid()
        {
            return To.IsValid() && From.IsValid();
        }
    }

    public struct Connection
    {
        private int _id;
        public int Id { get { return _id == default ? -1 : _id; } set { _id = value; } }
        public string ParameterName { get; set; }

        public bool IsValid()
        {
            return Id > 0 && !string.IsNullOrEmpty(ParameterName);
        }
    }


}
