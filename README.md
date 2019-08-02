# Capital City

Capital City is a city building and business simulation project. Inspired by the City Building Series developed under Impressions Games and Sierra, the player takes charge of a company town and runs the local industry. Proles move into houses and demand gradual improvements of life at pace with the rest of the world. The player hires these proles to work at mines or industrial sites or retail stores, etc.

Central to Capital City is the (social) labor theory of value. The value of a commodity as it is produced depends upon the average time necessary to produce 100 items of this commodity. Producing under this average time basically allows the player to generate more profit per commodity because it is being produced faster. The opposite situation occurs as well: the slower that a commodity is being produced, the more profit that the player is missing out on. Average production times are computed every financial quarter.

The main gameplay loop, besides town development, is to increase profits by producing faster than the social average. At the same time, as the average production time plummets, so does the value of the commodity being produced (being composed more and more of existing value from 'ingredients' like steel and coal from machinery). Transitioning from manual labor to automation decreases production times, but causes less value to be created because of the reliance on machinery. Increasing the length of the workday may compensate for producing below the social average time, but causes worker dissatisfaction and even workplace accidents (and costs more wages). 

## Current Plans

- [ ] Program new system for disease
  - Spread from person to person through workplaces and houses
- [ ] Add cemeteries
- [ ] Add orphanages for children to be held in after parent dies
  - Also: what happens if no orphanage is available?
  - Perhaps make it so that children can be sold to new parents at a price
- [ ] Possibly make new slot-based inventory system for storage buildings (NEED TO FIGURE OUT HOW TO QUEUE INCOMING ITEMS WITH INTO MULTIPLE SLOTS)
- [ ] Although we have a way of randomly killing proles, we should figure out HOW they died probably by rolling the relative weights of each manner of death (disease, age, work accident, etc.)
- [ ] Add random child birth (check every couple age intervals?)
- [ ] Figure out a good interval of time for proles to be aged up
  - One year takes too much in-game time
  - This may not be determined until we know how long an actual game lasts

## Building Ideas

- Bathhouse: Reduces waste output from houses
- Hospital: Heals disease
- Library: Provides entertainment, maybe improves education if that becomes a thing (req. Books)
- Gymnasium: Provides entertainment, maybe improves physical productivity
- Concert Hall: Provides entertainment (req. Instruments)
- Allotment Garden: Produces food from nearby workers' free time (= 16 - [Working Day])

## Mechanics Ideas

- Technology Research/Espionage (perhaps make other markdown file to describe)
- Productivity bonuses: fitness, education, anything else???
- Do children have special needs that adults/workers do not? Should education be provided by house or should children have to grow up with library access to be eductaed as an adult?

