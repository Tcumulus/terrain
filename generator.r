# https://www.firespark.de/resources/downloads/implementation%20of%20a%20methode%20for%20hydraulic%20erosion.pdf # nolint
# https://github.com/SebLague/Hydraulic-Erosion/blob/master/Assets/Scripts/Erosion.cs  # nolint

library(ambient)

# iterations
i_ <- 100000
lifetime_ <- 12

# parameters
width <- 100
height <- 100
frequency_ <- 0.025

# constants
erosion_ <- 0.3
deposition_ <- 0.3
evaporation_ <- 0.05
sediment_ <- 0.1
sedimentn_ <- 0.001
gravity <- 10

# initials
volume_ <- 0.01
speed_ <- 0.01


# generating perlin noise dataframe
noise_ <- noise_perlin(
  dim = c(height, width),
  frequency = frequency_,
  interpolator = "hermite",
  octaves = 4,
)

noise__ <- noise_ # temporary copy

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

      if (sediment < sedimentcap) {
        # calculate erosion amount
        erosion <- min((sedimentcap - sediment) * erosion_, -dh)

        # update height
        noise[x, y] <- noise[x, y] - erosion
        sediment <- sediment + erosion
      } else {
        # calculate deposition amount
        deposit <- (sediment - sedimentcap) * deposition_

        # check if neighboring pixel is higher
        if (dh < 0) {
          # update height
          noise[x, y] <- noise[x, y] - deposit
          sediment <- sediment + deposit
        } else {
          # check if sediment is bigger than delta height
          if (sediment < dh) {
            # stagnant: deposit all
            noise[x, y] <- noise[x, y] + sediment
            break # end loop
          } else {
            # deposit up to delta height
            deposit <- dh
            noise[x, y] <- noise[x, y] - deposit
            sediment <- sediment + deposit
          }
        }
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
  i <- i + 1
}

par(mfrow = c(1, 2))
plot(as.raster(normalise(noise__)))
plot(as.raster(normalise(noise_)))