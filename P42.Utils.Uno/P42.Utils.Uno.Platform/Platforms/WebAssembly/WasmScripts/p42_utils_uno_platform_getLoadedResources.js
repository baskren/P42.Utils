function P42_Utils_Uno_Platform_GetLoadedResources() {
    return JSON.stringify(performance.getEntriesByType("resource").map(r => r.name));
}
