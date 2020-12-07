public abstract class BuildableObject : Selectable
{
    public bool isBuilding = true;
    public abstract void HasBeenBuild();
    public abstract void wantsTobeBuild();
}
