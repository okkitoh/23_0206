#ifndef RAND
#define RAND


float dot(float2 a, float2 b) {
    return a.x*b.x+a.y*b.y;
}

#define N 624
#define M 397
#define MATRIX_A 0x9908b0df // constant vector a
#define UPPER_MASK 0x80000000 // most significant w-r bits
#define LOWER_MASK 0x7fffffff // least significant r bits

// Tempering parameters
#define TEMPERING_MASK_B 0x9d2c5680
#define TEMPERING_MASK_C 0xefc60000
#define TEMPERING_SHIFT_U(y) (y >> 11)
#define TEMPERING_SHIFT_S(y) (y << 7)
#define TEMPERING_SHIFT_T(y) (y << 15)
#define TEMPERING_SHIFT_L(y) (y >> 18)

// State vector
uniform uint mt[N];
uniform uint mti;

// Initialization
void mtwist_seed(uint seed) {
    mt[0] = seed;
    for (mti = 1; mti < N; ++mti) {
        mt[mti] = 1812433253U * (mt[mti - 1] ^ (mt[mti - 1] >> 30)) + mti;
    }
}

// Generate a 32-bit random number
uint mtwist_int32(uint seed) {
    mtwist_seed(seed);

    uint y;
    static const uint mag01[2] = {0, MATRIX_A};
    if (mti >= N) {
        for (int i = 0; i < N - M; ++i) {
            y = (mt[i] & UPPER_MASK) | (mt[i + 1] & LOWER_MASK);
            mt[i] = mt[i + M] ^ (y >> 1) ^ mag01[y & 1];
        }
        for (int i = N - M; i < N - 1; ++i) {
            y = (mt[i] & UPPER_MASK) | (mt[i + 1] & LOWER_MASK);
            mt[i] = mt[i + M - N] ^ (y >> 1) ^ mag01[y & 1];
        }
        y = (mt[N - 1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
        mt[N - 1] = mt[M - 1] ^ (y >> 1) ^ mag01[y & 1];
        mti = 0;
    }
    y = mt[mti++];
    y ^= TEMPERING_SHIFT_U(y);
    y ^= TEMPERING_SHIFT_S(y) & TEMPERING_MASK_B;
    y ^= TEMPERING_SHIFT_T(y) & TEMPERING_MASK_C;
    y ^= TEMPERING_SHIFT_L(y);
    return y;
}


// All of these are deterministic randomness no different than hash

// Generate a random float in the range [0, 1)
// Mersenne Twister
float mtwist(uint seed) {
    return mtwist_int32(seed) * (1.0f / 4294967296.0f);
}

// Generate random float in the range [0, 1)
float random(float2 n) {
    return frac(
        sin(
            dot(
               n, float2(12.9898, 78.233)
            )
        ) * 43758.5453123
    );
}

float random1(float seed) {
    return frac(sin(seed) * 143758.5453);
}

#endif