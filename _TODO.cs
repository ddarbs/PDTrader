
    /* pre-work // TODO:
     * - monitor chat, screenshot chat on new message and save to file
     * - ocr from saved screenshot w/tesseract, find keywords that they want to buy an item
     *      https://github.com/charlesw/tesseract-samples
     * - figure out what item they want from ocr
     * - tell them the price or not currently selling
     * - open and monitor the chest slots for 30s
     * - compare item and stack size to what we're expecting 
     *      https://stackoverflow.com/questions/35151067/algorithm-to-compare-two-images-in-c-sharp/35153895#35153895
     *      https://www.c-sharpcorner.com/UploadFile/prathore/image-comparison-using-C-Sharp/
     * - put item they're expecting in the chest
     * - back to monitoring chat
     */

    /* super late middle of the project // TODO:
     * - (done) get the cell rects for smallest chest can find
     * - (done) get pdtrader to clip a screenshot of empty cell, see if it will compare correctly across all cells
     * - (done) add rgb variation on image match
     * - (done) see how expensive it is to watch all cells, if too much just watch the first cell (top left)
     * - (done) detect when it's not empty
     * - (done) grab item to a dedicated inv cell we know is empty (use mouse drag code from WPFLearning or cheap out with double clicking like dump mode)
     * - (done) imagematch to item we're expecting (send back through the ImageMatchAPI but grab the quantity crop of new item this time too)
     * - (done) ocr quantity of grabbed item (using the quantity crop from previous step)
     * - (done) handling for invalid item/quantity
     * - (done) mash a full loop together for one item and one currency
     * - (done) flesh out the loop 
     *      - (done) get cell rects for player hotbar (while chest is open)
     *      - (done) swap from hard coded cells for empty/currency to constants in Library using player hotbar
     *      - (done) add chat options? for buying with more than steel ingots (see notes in Main greeting section)
     * - handling for
     *      - (done)cancelling transactions
     *      - (done)waiting too long for someone
     * - (done) swap from image matching to hover item and ocr item name text at the top
     * - (done) dynamic item handling for finding their purchase in bot inventory instead of hard coding where x item is
     *
     * - fix the stupid bug where it doesnt drag, seems like pax dei isn't registering the mouse move correctly rather than the mouse click not registering 
     * 
     * - (done) maybe swap ocr data from fast to accurate
     * - (done)scroll chat before reading new messages? (ingame bug with chat scroll causes it to ocr old messages)
     * - (partial) handling for multiple stacks/increased quantity?
     * - maybe swap ocr data back to fast? seems like it's trying too hard to find letters that aren't there
     */