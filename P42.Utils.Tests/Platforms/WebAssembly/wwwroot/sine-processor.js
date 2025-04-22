class SineProcessor extends AudioWorkletProcessor {
    constructor() {
        super();
        this.phase = 0;
        this.frequency = 440; // You can adjust this
        this.amplitude = 0.2;
    }

    process(inputs, outputs, parameters) {
        const output = outputs[0];
        output.forEach(channel => {
            for (let i = 0; i < channel.length; i++) {
                channel[i] = this.amplitude * Math.sin(2 * Math.PI * this.frequency * this.phase / sampleRate);
                this.phase++;
            }
        });
        return true;
    }
}

registerProcessor('sine-processor', SineProcessor);
