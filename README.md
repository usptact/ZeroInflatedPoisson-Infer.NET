# ZeroInflatedPoisson-Infer.NET
Simple demo of Zero-Inflated Poisson (ZIP) model using Infer.NET framework.

Zero inflated models can account for extra zeros. The generative story for each data point:
1. Flip a weighted coin
2. If heads, return zero
3. If tails, sample from Poisson distribution

The coin flip decides whether to return a zero. If tails, a sample from Poisson distribution can be also a zero. It is like deciding whether to go to a store and buy an item, one can still not buy (e.g. item is out of stock).

The Infer.NET framework has its own license.
