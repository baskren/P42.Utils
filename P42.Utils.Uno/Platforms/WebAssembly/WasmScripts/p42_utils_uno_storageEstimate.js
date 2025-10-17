async function P42_Utils_Uno_StorageEstimateJsonAsync() {
    const estimate = await navigator.storage.estimate();
    const result = {"usage": estimate.usage, "quota": estimate.quota};
    return JSON.stringify(result);
}

