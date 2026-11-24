package com.nsdey.minesweeper

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Canvas
import android.graphics.LinearGradient
import android.graphics.Paint
import android.graphics.Rect
import android.graphics.RectF
import android.graphics.Shader
import android.graphics.Typeface
import android.view.MotionEvent
import android.view.View
import android.widget.PopupMenu
import kotlin.math.floor
import kotlin.math.max
import kotlin.math.min

/** Fullscreen Minesweeper frame and empty portrait board, rendered using C# source proportions. */
class MinesweeperGameView(context: Context) : View(context) {
    private val density = resources.displayMetrics.density
    private val paint = Paint().apply {
        isAntiAlias = false
        isFilterBitmap = false
        style = Paint.Style.FILL
    }
    private val spritePaint = Paint().apply {
        isAntiAlias = false
        isFilterBitmap = false
        color = 0xFFFFFFFF.toInt()
    }
    private val counterTexture: Bitmap = BitmapFactory.decodeResource(resources, R.drawable.numbertexture)
    private val spaceTexture: Bitmap = BitmapFactory.decodeResource(resources, R.drawable.spacetextures)
    private val faceTexture: Bitmap = BitmapFactory.decodeResource(resources, R.drawable.facetexture)
    private val counterOffsets = intArrayOf(253, 230, 207, 184, 161, 138, 115, 92, 69, 46)
    private val numberOffsets = intArrayOf(112, 96, 80, 64, 48, 32, 16, 0)

    init {
        isFocusable = true
        isClickable = true
        contentDescription = "Minesweeper board"
        GameScale.observe(this)
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        val unit = GameScale.scaledPixels(context, 1f)
        val toolbarHeight = 56f * unit
        val sourceWidth = width / unit
        val sourceHeight = height / unit

        canvas.drawColor(COLOR_BACKGROUND)
        drawToolbar(canvas, unit, toolbarHeight)

        // MainWindow.cs reserves 12 source pixels on the left and 8 on the right
        // around a 16-pixel cell. Scale those measurements with every other asset.
        val columns = max(1, floor((sourceWidth - SOURCE_FORM_HORIZONTAL_EXTRA) / SOURCE_CELL).toInt())
        val rows = max(
            1,
            min(
                SOURCE_MAX_ROWS,
                floor((sourceHeight - SOURCE_TOOLBAR_HEIGHT - SOURCE_FIELD_TOP - SOURCE_FIELD_BOTTOM) / SOURCE_CELL).toInt()
            )
        )

        val boardX = SOURCE_FIELD_LEFT * unit
        val boardY = toolbarHeight + SOURCE_FIELD_TOP * unit
        val cellSize = SOURCE_CELL * unit
        val boardWidth = columns * cellSize
        val boardHeight = rows * cellSize
        val frameLeft = SOURCE_FRAME_LEFT * unit
        val frameRight = width - SOURCE_FRAME_RIGHT * unit
        val frameTop = toolbarHeight + SOURCE_FRAME_TOP * unit
        val frameBottom = boardY + boardHeight + SOURCE_FRAME_BOTTOM * unit

        // One raised frame contains two independent recessed wells: the head and the grid.
        drawRaisedPanel(canvas, 0f, toolbarHeight, width.toFloat(), frameBottom + SOURCE_FIELD_BOTTOM * unit, 2f * unit)
        drawInsetPanel(
            canvas,
            frameLeft,
            toolbarHeight + SOURCE_HEAD_TOP * unit,
            frameRight,
            toolbarHeight + SOURCE_HEAD_BOTTOM * unit,
            2f * unit
        )
        drawHead(canvas, unit, toolbarHeight)

        drawInsetPanel(canvas, frameLeft, frameTop, frameRight, frameBottom, 2f * unit)
        drawField(canvas, boardX, boardY, columns, rows, cellSize, boardWidth, boardHeight)
    }

    private fun drawToolbar(canvas: Canvas, unit: Float, toolbarHeight: Float) {
        fill(canvas, 0f, 0f, width.toFloat(), toolbarHeight, COLOR_BACKGROUND)
        fill(canvas, 0f, toolbarHeight - unit, width.toFloat(), toolbarHeight, COLOR_DARK)

        paint.color = COLOR_TEXT
        paint.typeface = Typeface.create("sans-serif", Typeface.BOLD)
        paint.textSize = 13f * unit
        paint.textAlign = Paint.Align.LEFT
        val baseline = toolbarHeight / 2f - (paint.ascent() + paint.descent()) / 2f
        canvas.drawText("Minesweeper", 8f * unit, baseline, paint)

        val centerX = width - 16f * unit
        for (index in 0..2) {
            val centerY = toolbarHeight / 2f + (index - 1) * 6f * unit
            paint.color = COLOR_TEXT
            canvas.drawCircle(centerX, centerY, 1.8f * unit, paint)
        }
    }

    private fun drawHead(canvas: Canvas, unit: Float, toolbarHeight: Float) {
        val counterY = toolbarHeight + 16f * unit
        val sourceWidth = width / unit
        val sourceFaceX = ((sourceWidth + 16f) / 2f) - 18f
        val sourceLeftFieldX = 17f
        val sourceRightFieldX = sourceWidth - 55f
        val fieldWidth = 41f * unit
        val faceFrameWidth = 26f * unit
        val preferredFaceX = sourceFaceX * unit
        val preferredLeftFrame = sourceLeftFieldX * unit
        val preferredRightFrame = sourceRightFieldX * unit
        val sourceLayoutFits =
            preferredLeftFrame + fieldWidth <= preferredFaceX - unit &&
                preferredFaceX + 25f * unit <= preferredRightFrame

        val leftDigitsX: Float
        val rightDigitsX: Float
        val faceFrameX: Float
        val faceX: Float
        if (sourceLayoutFits) {
            leftDigitsX = 18f * unit
            rightDigitsX = width - 54f * unit
            faceFrameX = preferredFaceX - unit
            faceX = preferredFaceX
        } else {
            // Preserve full-size sprites at large scales; compress only the gaps.
            val gap = ((width - 2f * fieldWidth - faceFrameWidth) / 4f).coerceAtLeast(0f)
            val leftFrameX = gap
            faceFrameX = leftFrameX + fieldWidth + gap
            val rightFrameX = faceFrameX + faceFrameWidth + gap
            leftDigitsX = leftFrameX + unit
            rightDigitsX = rightFrameX + unit
            faceX = faceFrameX + unit
        }

        drawCounter(canvas, leftDigitsX, counterY, 40, unit)
        drawCounter(canvas, rightDigitsX, counterY, 0, unit)
        val faceY = counterY
        drawRaisedPanel(canvas, faceFrameX, faceY - unit, faceFrameX + faceFrameWidth, faceY + 25f * unit, unit)
        drawSprite(faceTexture, Rect(0, 120, 24, 144), RectF(faceX, faceY, faceX + 24f * unit, faceY + 24f * unit), canvas)

    }

    private fun drawCounter(canvas: Canvas, startX: Float, y: Float, value: Int, unit: Float) {
        val digits = value.toString().padStart(3, '0').takeLast(3)
        for (index in digits.indices) {
            val digit = digits[index].digitToInt()
            val sourceTop = counterOffsets[digit]
            val x = startX + index * 13f * unit
            drawSprite(
                counterTexture,
                Rect(0, sourceTop, 13, sourceTop + 23),
                RectF(x, y, x + 13f * unit, y + 23f * unit),
                canvas
            )
        }
    }

    private fun drawField(
        canvas: Canvas,
        x: Float,
        y: Float,
        columns: Int,
        rows: Int,
        cellSize: Float,
        boardWidth: Float,
        boardHeight: Float
    ) {
        fill(canvas, x, y, x + boardWidth, y + boardHeight, COLOR_OPEN_FACE)

        // Match Minefield.cs: draw one #808080 rule per 16-pixel source cell.
        val unit = GameScale.scaledPixels(context, 1f)
        paint.color = COLOR_GRID
        paint.strokeWidth = unit
        for (column in 0..columns) {
            val lineX = x + column * cellSize
            canvas.drawLine(lineX, y, lineX, y + boardHeight, paint)
        }
        for (row in 0..rows) {
            val lineY = y + row * cellSize
            canvas.drawLine(x, lineY, x + boardWidth, lineY, paint)
        }

        // Covered buttons are generated at runtime; no fixed tile bitmap is used.
        for (row in 0 until rows) {
            for (column in 0 until columns) {
                val left = x + column * cellSize
                val top = y + row * cellSize
                drawClosedTile(canvas, left, top, cellSize)
            }
        }
    }

    /** Draws one of the number sprites with its original square aspect ratio. */
    private fun drawOpenNumber(canvas: Canvas, number: Int, left: Float, top: Float, cellSize: Float) {
        require(number in 1..8)
        val sourceTop = numberOffsets[number - 1]
        drawSprite(spaceTexture, Rect(0, sourceTop, 16, sourceTop + 16), RectF(left, top, left + cellSize, top + cellSize), canvas)
        drawOpenCellGrid(canvas, left, top, cellSize)
    }

    /** Draws an opened empty cell with the same single-pixel grid rule as numbered cells. */
    private fun drawOpenBlank(canvas: Canvas, left: Float, top: Float, cellSize: Float) {
        fill(canvas, left, top, left + cellSize, top + cellSize, COLOR_OPEN_FACE)
        drawOpenCellGrid(canvas, left, top, cellSize)
    }

    private fun drawOpenCellGrid(canvas: Canvas, left: Float, top: Float, cellSize: Float) {
        val edge = GameScale.scaledPixels(context, 1f)
        paint.color = COLOR_GRID
        canvas.drawRect(left, top, left + cellSize, top + edge, paint)
        canvas.drawRect(left, top + cellSize - edge, left + cellSize, top + cellSize, paint)
        canvas.drawRect(left, top, left + edge, top + cellSize, paint)
        canvas.drawRect(left + cellSize - edge, top, left + cellSize, top + cellSize, paint)
    }

    private fun drawClosedTile(canvas: Canvas, left: Float, top: Float, side: Float) {
        // Keep covered cells raised with a shallow one-pixel lip at the current scale.
        val edge = GameScale.scaledPixels(context, 1f)
        fill(canvas, left, top, left + side, top + side, COLOR_FACE)
        fill(canvas, left, top, left + side, top + edge, COLOR_WHITE)
        fill(canvas, left, top, left + edge, top + side, COLOR_WHITE)
        fill(canvas, left, top + side - edge, left + side, top + side, COLOR_DARK)
        fill(canvas, left + side - edge, top, left + side, top + side, COLOR_DARK)
    }

    private fun drawRaisedPanel(canvas: Canvas, left: Float, top: Float, right: Float, bottom: Float, edge: Float) {
        fill(canvas, left, top, right, bottom, COLOR_BACKGROUND)
        fillGradient(canvas, left, top, right, top + edge, COLOR_WHITE, COLOR_LIGHT, vertical = true)
        fillGradient(canvas, left, top, left + edge, bottom, COLOR_WHITE, COLOR_LIGHT, vertical = false)
        fillGradient(canvas, left, bottom - edge, right, bottom, COLOR_SHADOW, COLOR_DARK, vertical = true)
        fillGradient(canvas, right - edge, top, right, bottom, COLOR_SHADOW, COLOR_DARK, vertical = false)
    }

    private fun drawInsetPanel(canvas: Canvas, left: Float, top: Float, right: Float, bottom: Float, edge: Float) {
        fill(canvas, left, top, right, bottom, COLOR_BACKGROUND)
        fillGradient(canvas, left, top, right, top + edge, COLOR_DARK, COLOR_SHADOW, vertical = true)
        fillGradient(canvas, left, top, left + edge, bottom, COLOR_DARK, COLOR_SHADOW, vertical = false)
        fillGradient(canvas, left, bottom - edge, right, bottom, COLOR_LIGHT, COLOR_WHITE, vertical = true)
        fillGradient(canvas, right - edge, top, right, bottom, COLOR_LIGHT, COLOR_WHITE, vertical = false)
    }

    private fun fillGradient(
        canvas: Canvas,
        left: Float,
        top: Float,
        right: Float,
        bottom: Float,
        startColor: Int,
        endColor: Int,
        vertical: Boolean
    ) {
        if (right <= left || bottom <= top) return
        val endX = if (vertical) left else right
        val endY = if (vertical) bottom else top
        paint.shader = LinearGradient(left, top, endX, endY, startColor, endColor, Shader.TileMode.CLAMP)
        canvas.drawRect(left, top, right, bottom, paint)
        paint.shader = null
    }

    private fun drawSprite(bitmap: Bitmap, source: Rect, destination: RectF, canvas: Canvas) {
        canvas.drawBitmap(bitmap, source, destination, spritePaint)
    }

    private fun fill(canvas: Canvas, left: Float, top: Float, right: Float, bottom: Float, color: Int) {
        if (right <= left || bottom <= top) return
        paint.color = color
        canvas.drawRect(left, top, right, bottom, paint)
    }

    override fun onTouchEvent(event: MotionEvent): Boolean {
        val unit = GameScale.scaledPixels(context, 1f)
        if (event.actionMasked == MotionEvent.ACTION_UP && event.y <= 56f * unit && event.x >= width - 48f * unit) {
            PopupMenu(context, this).apply {
                menu.add("New game")
                menu.add("Settings")
                show()
            }
            performClick()
            return true
        }
        return true
    }

    override fun performClick(): Boolean {
        super.performClick()
        return true
    }

    override fun onDetachedFromWindow() {
        GameScale.stopObserving(this)
        super.onDetachedFromWindow()
    }

    private companion object {
        const val SOURCE_CELL = 16f
        const val SOURCE_MAX_ROWS = 50
        const val SOURCE_FORM_HORIZONTAL_EXTRA = 20f
        const val SOURCE_TOOLBAR_HEIGHT = 56f
        const val SOURCE_FIELD_LEFT = 12f
        const val SOURCE_FIELD_TOP = 55f
        const val SOURCE_FIELD_BOTTOM = 19f
        const val SOURCE_FRAME_LEFT = 9f
        const val SOURCE_FRAME_RIGHT = 6f
        const val SOURCE_FRAME_TOP = 52f
        const val SOURCE_FRAME_BOTTOM = 3f
        const val SOURCE_HEAD_TOP = 9f
        const val SOURCE_HEAD_BOTTOM = 45f

        const val COLOR_WHITE = 0xFFFFFFFF.toInt()
        const val COLOR_LIGHT = 0xFFDFDFDF.toInt()
        const val COLOR_BACKGROUND = 0xFFC0C0C0.toInt()
        const val COLOR_FACE = 0xFFBDBDBD.toInt()
        const val COLOR_OPEN_FACE = 0xFFC0C0C0.toInt()
        const val COLOR_SHADOW = 0xFF9A9A9A.toInt()
        const val COLOR_DARK = 0xFF808080.toInt()
        const val COLOR_GRID = 0xFF808080.toInt()
        const val COLOR_TEXT = 0xFF202020.toInt()
    }
}
