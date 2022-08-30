namespace FRSDeletePOC
{
    public record FRSContainers(FRSContainer[] Value);
    public record FRSContainer(string Id, string Name, string Type, FRSContainerProperties Properties);

    public record FRSContainerProperties (string FRSContainerId, string FRSTenantId, DateTime CreationTime, DateTime LastAccessTime);
}
