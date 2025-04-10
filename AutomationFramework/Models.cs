namespace AutomationFramework
{
    public enum VialType {
        Empty = 0,
        Insulin
    }

    public class Vial
    {
        public VialRack? Rack { get; set; }
        public VialType? Type { get; set; }
    }

    public class VialRack
    {
        public Mover? Mover;
    }

    public class Mover
    {
        public int? Id;
    }

    public class Station
    {
        public string? Name { get; set; }

        public virtual void Start() { }
    }
}
