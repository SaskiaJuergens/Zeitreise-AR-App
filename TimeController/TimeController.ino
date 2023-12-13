#include <Arduino_GFX_Library.h>
#include <TouchScreen.h>

//Chip ESP32 S3
#define TFT_BLK 45
#define TFT_RES 11

#define TFT_CS 15
#define TFT_MOSI 13
#define TFT_MISO 12
#define TFT_SCLK 14
#define TFT_DC 21

#define GFX_BL TFT_BLK

//Display Initialization
Arduino_ESP32SPI *bus = new Arduino_ESP32SPI(TFT_DC, TFT_CS, TFT_SCLK, TFT_MOSI, TFT_MISO, HSPI, true); // Constructor
Arduino_GFX *gfx = new Arduino_GC9A01(bus, TFT_RES, 0 /* rotation */, true /* IPS */);

#define BACKGROUND WHITE
#define MARK_COLOR BLACK
#define SUBMARK_COLOR DARKGREY 
#define HOUR_COLOR BLACK
#define MINUTE_COLOR BLACK
#define SECOND_COLOR BLACK

#define SIXTIETH 0.016666667
#define TWELFTH 0.08333333
#define SIXTIETH_RADIAN 0.10471976
#define TWELFTH_RADIAN 0.52359878
#define RIGHT_ANGLE_RADIAN 1.5707963

// Touchscreen-Pin-Definitionen (Anpassung an deine tatsächliche Hardware erforderlich)
#define TS_CS 22
#define TS_IRQ 23
TouchScreen ts = TouchScreen(TS_CS, TS_IRQ, 0, 0, 0);


static uint8_t conv2d(const char *p)
{
    uint8_t v = 0;
    return (10 * (*p - '0')) + (*++p - '0');
}

static int16_t w, h, center;
static int16_t hHandLen, mHandLen, sHandLen, markLen;
static float sdeg, mdeg, hdeg;
static int16_t osx = 0, osy = 0, omx = 0, omy = 0, ohx = 0, ohy = 0; // Saved H, M, S x & y coords
static int16_t nsx, nsy, nmx, nmy, nhx, nhy;                         // H, M, S x & y coords
static int16_t xMin, yMin, xMax, yMax;                               // redraw range
static int16_t hh, mm, ss;
static unsigned long targetTime; // next action time

static int16_t *cached_points;
static uint16_t cached_points_idx = 0;
static int16_t *last_cached_point;

void setup(void)
{
    gfx->begin();
    gfx->fillScreen(BACKGROUND);

#ifdef GFX_BL
    pinMode(GFX_BL, OUTPUT);
    digitalWrite(GFX_BL, HIGH);
#endif

    // init LCD constant
    w = gfx->width();
    h = gfx->height();
    if (w < h)
    {
        center = w / 2;
    }
    else
    {
        center = h / 2;
    }
    hHandLen = center * 3 / 8;
    mHandLen = center * 2 / 3;
    sHandLen = center * 5 / 6;
    markLen = sHandLen / 6;
    cached_points = (int16_t *)malloc((hHandLen + 1 + mHandLen + 1 + sHandLen + 1) * 2 * 2);

    // Draw Roman numeral for 12
    draw_roman_clock_mark(0, center - markLen * 3, center - markLen * 4);

    // Draw Roman numerals for 1 to 11
    for (int i = 1; i <= 11; i++) {
        draw_roman_clock_mark(i, center - markLen * 2, center - markLen * 3);
    }

        hh = conv2d(__TIME__);
    mm = conv2d(__TIME__ + 3);
    ss = conv2d(__TIME__ + 6);

    targetTime = ((millis() / 1000) + 1) * 1000;
}

void draw_hands(int16_t x, int16_t y, float angle, int16_t length, int16_t width, int16_t color)
{
    // Calculate the endpoint of the hand
    int16_t x2 = x + cos(angle) * length;
    int16_t y2 = y + sin(angle) * length;

    // Calculate the two sides of the hand to create a triangle (arrowhead)
    int16_t x3 = x2 + cos(angle + PI / 10) * width;
    int16_t y3 = y2 + sin(angle + PI / 10) * width;
    int16_t x4 = x2 + cos(angle - PI / 10) * width;
    int16_t y4 = y2 + sin(angle - PI / 10) * width;

    // Draw the hand (line) and arrowhead (triangle)
    gfx->drawLine(x, y, x2, y2, color);
    gfx->fillTriangle(x2, y2, x3, y3, x4, y4, color);
}

//Römische Zahlen für die UI
void draw_roman_clock_mark(int16_t hour, int16_t outerR, int16_t innerR)
{
    String romanNumerals[] = {"XII", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI"};

    float mdeg = (SIXTIETH_RADIAN * hour * 5) - RIGHT_ANGLE_RADIAN;

    int16_t x0 = cos(mdeg) * outerR + center;
    int16_t y0 = sin(mdeg) * outerR + center;
    int16_t x1 = cos(mdeg) * innerR + center;
    int16_t y1 = sin(mdeg) * innerR + center;

    // Berechne die Position für die römische Zahl
    int16_t romanX = (x0 + x1) / 2;
    int16_t romanY = (y0 + y1) / 2;

    // Draw Roman numeral
    //gfx->setCursor((x0 + x1) / 2 - 10, (y0 + y1) / 2 - 5);
    gfx->setCursor(romanX - 10, romanY - 10);
    //Zahlengröße
    gfx->setTextSize(2);
    gfx->setTextColor(MARK_COLOR);
    gfx->print(romanNumerals[hour]);

    // Draw clock mark
    gfx->drawLine(x0, y0, x1, y1, MARK_COLOR);
}



void loop()
{
    unsigned long cur_millis = millis();
      TSPoint touch = ts.getPoint();
  
  Serial.print("X: "); Serial.print(touch.x);
  Serial.print(" Y: "); Serial.print(touch.y);
  Serial.print(" Z: "); Serial.println(touch.z);

        // Zeichne zuerst die römischen Zahlen
        // Draw Roman numeral for 12
        draw_roman_clock_mark(0, center - markLen * 3, center - markLen * 4);

        // Draw Roman numerals for 1 to 11
        for (int i = 1; i <= 11; i++)
        {
            draw_roman_clock_mark(i, center - markLen * 2, center - markLen * 3);
        }

        // Dann die normalen Uhrmarkierungen
        draw_round_clock_mark(center - markLen * 3, center - markLen * 4, center - markLen * 2, center - markLen * 3, center - markLen, center - markLen * 2);


    if (touch.z > 0 && touch.x >= 0 && touch.x <= w && touch.y >= 0 && touch.y <= h)
    {
        // Touchscreen-Eingabe verarbeiten, große Zeigerposition aktualisieren
        nhx = touch.x;
        nhy = touch.y;
    }
    else
    {
        // Reguläre Uhr-Logik nur dann ausführen, wenn der Bildschirm nicht berührt wird
        if (cur_millis >= targetTime)
        {
            targetTime += 1000;
            ss++;

            if (ss == 60)
            {
                ss = 0;
                mm++;

                if (mm > 59)
                {
                    mm = 0;
                    hh++;

                    if (hh > 23)
                    {
                        hh = 0;
                    }
                }
            }
        }

        // Hier die restliche Uhr-Logik ausführen (sHand, mHand, hHand berechnen und zeichnen)
        sdeg = SIXTIETH_RADIAN * ((0.001 * (cur_millis % 1000)) + ss);
        nsx = cos(sdeg - RIGHT_ANGLE_RADIAN) * sHandLen + center;
        nsy = sin(sdeg - RIGHT_ANGLE_RADIAN) * sHandLen + center;

        mdeg = (SIXTIETH * sdeg) + (SIXTIETH_RADIAN * mm);
        hdeg = (TWELFTH * mdeg) + (TWELFTH_RADIAN * hh);
        mdeg -= RIGHT_ANGLE_RADIAN;
        hdeg -= RIGHT_ANGLE_RADIAN;
        nmx = cos(mdeg) * mHandLen + center;
        nmy = sin(mdeg) * mHandLen + center;
        nhx = cos(hdeg) * hHandLen + center;
        nhy = sin(hdeg) * hHandLen + center;


        gfx->startWrite();
        draw_and_erase_cached_line(center, center, nsx, nsy, SECOND_COLOR, cached_points, sHandLen + 1, false, false);
        draw_and_erase_cached_line(center, center, nhx, nhy, HOUR_COLOR, cached_points + ((sHandLen + 1) * 2), hHandLen + 1, true, false);
        draw_and_erase_cached_line(center, center, nmx, nmy, MINUTE_COLOR, cached_points + ((sHandLen + 1 + hHandLen + 1) * 2), mHandLen + 1, true, true);
        gfx->endWrite();

    }

    delay(10); // Hinzugefügte Verzögerung
}


void draw_round_clock_mark(int16_t innerR1, int16_t outerR1, int16_t innerR2, int16_t outerR2, int16_t innerR3, int16_t outerR3)
{
    float x, y;
    int16_t x0, x1, y0, y1, innerR, outerR;
    uint16_t c;

    for (uint8_t i = 0; i < 60; i++)
    {
        if ((i % 15) == 0)
        {
            innerR = innerR1;
            outerR = outerR1;
            c = MARK_COLOR;
        }
        else if ((i % 5) == 0)
        {
            innerR = innerR2;
            outerR = outerR2;
            c = MARK_COLOR;
        }
        else
        {
            innerR = innerR3;
            outerR = outerR3;
            c = SUBMARK_COLOR;
        }

        mdeg = (SIXTIETH_RADIAN * i) - RIGHT_ANGLE_RADIAN;
        x = cos(mdeg);
        y = sin(mdeg);
        x0 = x * outerR + center;
        y0 = y * outerR + center;
        x1 = x * innerR + center;
        y1 = y * innerR + center;

        // Verkürze die äußeren Marker
        float factor = 1.1; // Du kannst den Faktor entsprechend anpassen
        x0 = center + (x * (outerR * factor));
        y0 = center + (y * (outerR * factor));

        gfx->drawLine(x0, y0, x1, y1, c);
    }
}

void draw_square_clock_mark(int16_t innerR1, int16_t outerR1, int16_t innerR2, int16_t outerR2, int16_t innerR3, int16_t outerR3)
{
    float x, y;
    int16_t x0, x1, y0, y1, innerR, outerR;
    uint16_t c;

    for (uint8_t i = 0; i < 60; i++)
    {
        if ((i % 15) == 0)
        {
            innerR = innerR1;
            outerR = outerR1;
            c = MARK_COLOR;
        }
        else if ((i % 5) == 0)
        {
            innerR = innerR2;
            outerR = outerR2;
            c = MARK_COLOR;
        }
        else
        {
            innerR = innerR3;
            outerR = outerR3;
            c = SUBMARK_COLOR;
        }

        if ((i >= 53) || (i < 8))
        {
            x = tan(SIXTIETH_RADIAN * i);
            x0 = center + (x * outerR);
            y0 = center + (1 - outerR);
            x1 = center + (x * innerR);
            y1 = center + (1 - innerR);
        }
        else if (i < 23)
        {
            y = tan((SIXTIETH_RADIAN * i) - RIGHT_ANGLE_RADIAN);
            x0 = center + (outerR);
            y0 = center + (y * outerR);
            x1 = center + (innerR);
            y1 = center + (y * innerR);
        }
        else if (i < 38)
        {
            x = tan(SIXTIETH_RADIAN * i);
            x0 = center - (x * outerR);
            y0 = center + (outerR);
            x1 = center - (x * innerR);
            y1 = center + (innerR);
        }
        else if (i < 53)
        {
            y = tan((SIXTIETH_RADIAN * i) - RIGHT_ANGLE_RADIAN);
            x0 = center + (1 - outerR);
            y0 = center - (y * outerR);
            x1 = center + (1 - innerR);
            y1 = center - (y * innerR);
        }
        gfx->drawLine(x0, y0, x1, y1, c);
    }
}

void redraw_hands_cached_draw_and_erase()
{
    gfx->startWrite();
    draw_and_erase_cached_line(center, center, nsx, nsy, SECOND_COLOR, cached_points, sHandLen + 1, false, false);
    draw_and_erase_cached_line(center, center, nhx, nhy, HOUR_COLOR, cached_points + ((sHandLen + 1) * 2), hHandLen + 1, true, false);
    draw_and_erase_cached_line(center, center, nmx, nmy, MINUTE_COLOR, cached_points + ((sHandLen + 1 + hHandLen + 1) * 2), mHandLen + 1, true, true);
    gfx->endWrite();
}

void draw_and_erase_cached_line(int16_t x0, int16_t y0, int16_t x1, int16_t y1, int16_t color, int16_t *cache, int16_t cache_len, bool cross_check_second, bool cross_check_hour)
{
#if defined(ESP8266)
    yield();
#endif
    bool steep = _diff(y1, y0) > _diff(x1, x0);
    if (steep)
    {
        _swap_int16_t(x0, y0);
        _swap_int16_t(x1, y1);
    }

    int16_t dx, dy;
    dx = _diff(x1, x0);
    dy = _diff(y1, y0);

    int16_t err = dx / 2;
    int8_t xstep = (x0 < x1) ? 1 : -1;
    int8_t ystep = (y0 < y1) ? 1 : -1;
    x1 += xstep;
    int16_t x, y, ox, oy;
    for (uint16_t i = 0; i <= dx; i++)
    {
        if (steep)
        {
            x = y0;
            y = x0;
        }
        else
        {
            x = x0;
            y = y0;
        }
        ox = *(cache + (i * 2));
        oy = *(cache + (i * 2) + 1);
        if ((x == ox) && (y == oy))
        {
            if (cross_check_second || cross_check_hour)
            {
                write_cache_pixel(x, y, color, cross_check_second, cross_check_hour);
            }
        }
        else
        {
            write_cache_pixel(x, y, color, cross_check_second, cross_check_hour);
            if ((ox > 0) || (oy > 0))
            {
                write_cache_pixel(ox, oy, BACKGROUND, cross_check_second, cross_check_hour);
            }
            *(cache + (i * 2)) = x;
            *(cache + (i * 2) + 1) = y;
        }
        if (err < dy)
        {
            y0 += ystep;
            err += dx;
        }
        err -= dy;
        x0 += xstep;
    }
    for (uint16_t i = dx + 1; i < cache_len; i++)
    {
        ox = *(cache + (i * 2));
        oy = *(cache + (i * 2) + 1);
        if ((ox > 0) || (oy > 0))
        {
            write_cache_pixel(ox, oy, BACKGROUND, cross_check_second, cross_check_hour);
        }
        *(cache + (i * 2)) = 0;
        *(cache + (i * 2) + 1) = 0;
    }
}

void write_cache_pixel(int16_t x, int16_t y, int16_t color, bool cross_check_second, bool cross_check_hour)
{
    int16_t *cache = cached_points;
    if (cross_check_second)
    {
        for (uint16_t i = 0; i <= sHandLen; i++)
        {
            if ((x == *(cache++)) && (y == *(cache)))
            {
                return;
            }
            cache++;
        }
    }
    if (cross_check_hour)
    {
        cache = cached_points + ((sHandLen + 1) * 2);
        for (uint16_t i = 0; i <= hHandLen; i++)
        {
            if ((x == *(cache++)) && (y == *(cache)))
            {
                return;
            }
            cache++;
        }
    }
    gfx->writePixel(x, y, color);
}
