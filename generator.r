# https://www.firespark.de/resources/downloads/implementation%20of%20a%20methode%20for%20hydraulic%20erosion.pdf

library(ambient)

wi <- 50 # width
hi <- 50 # height
c_ <- 0.01 # erosion constant

# generating perlin noise dataframe
noise <- noise_perlin(
  dim = c(hi, wi),
  frequency = 0.05, # 0.005 for 500,500
  interpolator = "hermite",
  octaves = 4,
)

noise_ <- noise # temporary copy
i <- 0

# iterations
while (i < 100) {
  # looping through all pixels
  for (j in seq(ncol(noise))) {
    for (k in seq(nrow(noise))) {

      # TODO: water fixel continuation

      # checking if pixel is in scope
      if (k > 1 & k < wi & j > 1 & j < hi) {

        # get height of current pixel
        h <- noise[k, j]

        # get delta height of all surounding pixels
        d1 <- h - noise[k - 1, j] # left
        d2 <- h - noise[k, j + 1] # top
        d3 <- h - noise[k + 1, j] # right
        d4 <- h - noise[k, j + 1] # bottom

        # slope: max delta height difference (downwards)
        d <- max(d1, d2, d3, d4)

        # erosion amount
        c <- d * c_

        # substract erosion amount from pixel
        noise[k, j] <- h - c
      }
    }
  }
  i <- i + 1
}

par(mfrow = c(1, 2))
plot(as.raster(normalise(noise_)))
plot(as.raster(normalise(noise)))