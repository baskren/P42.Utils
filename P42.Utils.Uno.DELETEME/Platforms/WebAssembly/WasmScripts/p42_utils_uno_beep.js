function P42_Utils_Uno_Beep(frequency, duration) {
    console.log("BEEP ENTER");
    let context = new (window.AudioContext || window.webkitAudioContext)();
    let osc = context.createOscillator(); // instantiate an oscillator
    osc.type = 'sine'; // this is the default - also square, sawtooth, triangle
    osc.frequency.value = frequency; // Hz
    osc.connect(context.destination); // connect it to the destination
    osc.start(); // start the oscillator
    osc.stop(context.currentTime + duration/1000.0);
    console.log("BEEP EXIT");
}
