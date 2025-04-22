console.log('p42_utils_uno_A00_beep_processor [LOADING ENTER]');


export default class p42_utils_uno_A00_beep_processor extends AudioWorkletProcessor {
    constructor(options) {
        super();

        // Get the processor options
        const { frequency, duration } = options.processorOptions;

        // Initialize state
        this.frequency = frequency || 440;
        this.duration = duration || 1000;
        this.sampleRate = sampleRate;
        this.phase = 0;
        this.samplesProcessed = 0;

        // Calculate the number of samples for the duration
        this.totalSamples = Math.round((this.duration / 1000) * this.sampleRate);
    }

    process(inputs, outputs, parameters) {
        console.log('p42_utils_uno_A00_beep_processor [ENTER]');
        const output = outputs[0];
        const channel = output[0];

        console.log('p42_utils_uno_A00_beep_processor [A]');
        for (let i = 0; i < channel.length; i++) {
            if (this.samplesProcessed < this.totalSamples) {
                channel[i] = Math.sin(this.phase);
                this.phase += (2 * Math.PI * this.frequency) / this.sampleRate;

                // Keep phase within the range of 0 to 2*PI
                if (this.phase >= 2 * Math.PI) this.phase -= 2 * Math.PI;

                this.samplesProcessed++;
            } else {
                channel[i] = 0; // Silence after the tone
            }
        }
        console.log('p42_utils_uno_A00_beep_processor [EXIT]');

        // Stop processing once the tone is complete
        return this.samplesProcessed < this.totalSamples;
    }
}

registerProcessor('p42_utils_uno_A00_beep_processor', p42_utils_uno_A00_beep_processor);

console.log('p42_utils_uno_A00_beep_processor [LOADING EXIT]');
