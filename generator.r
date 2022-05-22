# https://www.firespark.de/resources/downloads/implementation%20of%20a%20methode%20for%20hydraulic%20erosion.pdf # nolint
# https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs  # nolint

library(ambient)

# iterations
i_ <- 50000
lifetime_ <- 15

# parameters
width <- 100
height <- 100
frequency_ <- 0.01

# constants
erosion_ <- 0.3
deposition_ <- 0.3
evaporation_ <- 0.02
sediment_ <- 1
sedimentn_ <- 0.01
gravity <- 4

# initials
volume_ <- 0.01
speed_ <- 1


# generating perlin noise dataframe
noise_ <- noise_perlin(
  dim = c(height, width),
  frequency = frequency_,
  interpolator = "hermite",
  octaves = 4
)

# TODO: masking

noise__ <- noise_ # temporary copy
noise3 <- noise_
noise4 <- noise_
noise5 <- noise_

# main function
erode <- function(noise) {

  # select random pixel
  x <- sample(1:width, 1)
  y <- sample(1:height, 1)

  # initialize
  speed <- speed_
  volume <- volume_
  sediment <- 0

  # get height of pixel
  h <- noise[x, y]

  # loop for the dropplet
  lifetime <- 0
  while (lifetime < lifetime_) {

    # check if in scope
    if (x > 1 & y > 1 & x < width & y < height) {

      # calculate height difference for the 4 neighboring pixels
      dh_w <- noise[x - 1, y] - h # left (west)
      dh_n <- noise[x, y - 1] - h # top (north)
      dh_e <- noise[x + 1, y] - h # right (east)
      dh_s <- noise[x, y + 1] - h # bottom (south)

      # get the largest height difference
      dh <- min(dh_w, dh_n, dh_e, dh_s)

      # calculate sediment cap
      sedimentcap <- max(-dh * speed * volume * sediment_, sedimentn_)

      if (sediment > sedimentcap | dh > 0) {
        # calculate deposition amount, if flowing uphill: fill up to height
        deposit <- if (dh > 0) min(dh, sediment) else (sediment - sedimentcap) * deposition_ # nolint
        deposit <- if (lifetime == lifetime_ - 1) sediment else deposit

        # update height
        noise[x, y] <- noise[x, y] - deposit
        sediment <- sediment + deposit
      } else {
        # calculate erosion amount
        erosion <- min((sedimentcap - sediment) * erosion_, -dh)

        # update height of current pixel
        noise[x, y] <- noise[x, y] - erosion
        sediment <- sediment + erosion
      }

      # temporary limits for infinite holes bug
      if (noise[x, y] < -1) {
        noise[x, y] <- -1
      } else if (noise[x, y] > 1) {
        noise[x, y] <- 1
      }

      # update x, y for new pixel
      if (dh == dh_w) {
        x <- x - 1
      } else if (dh == dh_n) {
        y <- y - 1
      } else if (dh == dh_e) {
        x <- x + 1
      } else {
        y <- y + 1
      }

      # update speed and volume
      volume <- volume * (1 - evaporation_)
      if (dh >= 0) {
        speed <- speed_
      } else {
        speed <- sqrt(speed**2 - dh * gravity)
      }
    }
    lifetime <- lifetime + 1
  }
  return(noise)
}


# iterations
i <- 0
while (i < i_) {
  noise_ <- erode(noise_)
  if (i == 2500) {
    noise3 <- noise_
  }
  if (i == 5000) {
    noise4 <- noise_
  }
  if (i == 7500) {
    noise5 <- noise_
  }
  i <- i + 1
}

par(mfrow = c(2, 3))
plot(as.raster(normalise(noise__)))
plot(as.raster(normalise(noise3)))
plot(as.raster(normalise(noise4)))
plot(as.raster(normalise(noise5)))
plot(as.raster(normalise(noise_)))