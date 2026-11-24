package com.nsdey.minesweeper

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Canvas
import android.graphics.Paint
import android.graphics.Rect
import android.graphics.RectF
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View

/** A single Minesweeper tile, drawn at runtime in the classic raised/recessed style. */
class MinesweeperTileView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : View(context, attrs, defStyleAttr) {

    enum class State {
        CLOSED,
        PRESSED,
        OPEN
    }

    var state: State = State.CLOSED
        set(value) {
            if (field != value) {
                field = value
                updateDescription()
                invalidate()
            }
        }

    /** Set to 1..8 to show a number on an open tile; null leaves it blank. */
    var number: Int? = null
        set(value) {
            require(value == null || value in 1..8) { "Tile number must be between 1 and 8." }
            if (field != value) {
                field = value
                updateDescription()
                invalidate()
            }
        }

    /** Called after a closed tile is tapped. The board decides how to reveal it. */
    var onTileClick: (() -> Unit)? = null

    private val paint = Paint().apply {
        style = Paint.Style.FILL
        isAntiAlias = false
    }
    private val spritePaint = Paint().apply {
        isAntiAlias = false
        isFilterBitmap = false
    }
    private val spaceTexture: Bitmap = BitmapFactory.decodeResource(resources, R.drawable.spacetextures)
    private val numberOffsets = intArrayOf(112, 96, 80, 64, 48, 32, 16, 0)
    private var pressedFromClosed = false
    private val bounds = RectF()

    init {
        isClickable = true
        isFocusable = true
        GameScale.observe(this)
        updateDescription()
    }

    override fun onMeasure(widthMeasureSpec: Int, heightMeasureSpec: Int) {
        val baseCell = GameScale.scaledPixels(context, 16f).toInt()
        setMeasuredDimension(
            resolveSize(baseCell, widthMeasureSpec),
            resolveSize(baseCell, heightMeasureSpec)
        )
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)
        val side = minOf(width, height).toFloat()
        if (side <= 0f) return

        // Keep the tile square and centered even if its parent gives it extra space.
        val left = (width - side) / 2f
        val top = (height - side) / 2f
        bounds.set(left, top, left + side, top + side)
        val edge = (side * 0.10f).coerceAtLeast(GameScale.scaledPixels(context, 1f))

        when (state) {
            State.CLOSED -> drawRaised(canvas, edge)
            State.PRESSED -> drawPressed(canvas, edge)
            State.OPEN -> drawOpen(canvas)
        }

        if (state == State.OPEN) drawNumber(canvas)
    }

    private fun drawRaised(canvas: Canvas, edge: Float) {
        fill(canvas, bounds, COLOR_FACE)
        fill(canvas, bounds.left, bounds.top, bounds.right, bounds.top + edge, COLOR_WHITE)
        fill(canvas, bounds.left, bounds.top, bounds.left + edge, bounds.bottom, COLOR_WHITE)
        fill(canvas, bounds.left + edge, bounds.top + edge, bounds.right, bounds.top + edge * 2, COLOR_LIGHT)
        fill(canvas, bounds.left + edge, bounds.top + edge, bounds.left + edge * 2, bounds.bottom, COLOR_LIGHT)
        fill(canvas, bounds.left, bounds.bottom - edge, bounds.right, bounds.bottom, COLOR_DARK)
        fill(canvas, bounds.right - edge, bounds.top, bounds.right, bounds.bottom, COLOR_DARK)
        fill(canvas, bounds.left + edge, bounds.bottom - edge * 2, bounds.right - edge, bounds.bottom - edge, COLOR_DARK)
        fill(canvas, bounds.right - edge * 2, bounds.top + edge, bounds.right - edge, bounds.bottom - edge, COLOR_DARK)
    }

    private fun drawPressed(canvas: Canvas, edge: Float) {
        fill(canvas, bounds, COLOR_FACE)
        fill(canvas, bounds.left, bounds.top, bounds.right, bounds.top + edge, COLOR_DARK)
        fill(canvas, bounds.left, bounds.top, bounds.left + edge, bounds.bottom, COLOR_DARK)
        fill(canvas, bounds.left + edge, bounds.top + edge, bounds.right - edge, bounds.bottom - edge, COLOR_FACE)
        fill(canvas, bounds.left + edge, bounds.bottom - edge, bounds.right, bounds.bottom, COLOR_LIGHT)
        fill(canvas, bounds.right - edge, bounds.top + edge, bounds.right, bounds.bottom, COLOR_LIGHT)
    }

    private fun drawOpen(canvas: Canvas) {
        fill(canvas, bounds, COLOR_OPEN_FACE)

        // Each tile contributes half of a line to each edge, so neighboring open
        // tiles join into a crisp one-dp grid without doubled seams.
        val halfGrid = GameScale.scaledPixels(context, 0.5f).coerceAtLeast(0.5f)
        fill(canvas, bounds.left, bounds.top, bounds.right, bounds.top + halfGrid, COLOR_GRID)
        fill(canvas, bounds.left, bounds.top, bounds.left + halfGrid, bounds.bottom, COLOR_GRID)
        fill(canvas, bounds.left, bounds.bottom - halfGrid, bounds.right, bounds.bottom, COLOR_GRID)
        fill(canvas, bounds.right - halfGrid, bounds.top, bounds.right, bounds.bottom, COLOR_GRID)
    }

    private fun drawNumber(canvas: Canvas) {
        val value = number ?: return
        val sourceTop = numberOffsets[value - 1]
        canvas.drawBitmap(spaceTexture, Rect(0, sourceTop, 16, sourceTop + 16), bounds, spritePaint)
    }

    override fun onTouchEvent(event: MotionEvent): Boolean {
        if (!isEnabled) return false
        when (event.actionMasked) {
            MotionEvent.ACTION_DOWN -> {
                pressedFromClosed = state == State.CLOSED
                if (pressedFromClosed) state = State.PRESSED
                return true
            }
            MotionEvent.ACTION_MOVE -> {
                if (pressedFromClosed) {
                    state = if (contains(event.x, event.y)) State.PRESSED else State.CLOSED
                }
                return true
            }
            MotionEvent.ACTION_UP -> {
                val shouldClick = pressedFromClosed && contains(event.x, event.y)
                if (pressedFromClosed) state = State.CLOSED
                pressedFromClosed = false
                if (shouldClick) performClick()
                return true
            }
            MotionEvent.ACTION_CANCEL -> {
                if (pressedFromClosed) state = State.CLOSED
                pressedFromClosed = false
                return true
            }
        }
        return super.onTouchEvent(event)
    }

    override fun performClick(): Boolean {
        super.performClick()
        onTileClick?.invoke()
        return true
    }

    override fun onDetachedFromWindow() {
        GameScale.stopObserving(this)
        super.onDetachedFromWindow()
    }

    private fun contains(x: Float, y: Float): Boolean = x >= 0f && y >= 0f && x < width && y < height

    private fun updateDescription() {
        contentDescription = when (state) {
            State.CLOSED -> "Closed tile"
            State.PRESSED -> "Tile pressed"
            State.OPEN -> number?.let { "Open tile, $it" } ?: "Open tile"
        }
    }

    private fun fill(canvas: Canvas, rect: RectF, color: Int) {
        paint.color = color
        canvas.drawRect(rect, paint)
    }

    private fun fill(canvas: Canvas, left: Float, top: Float, right: Float, bottom: Float, color: Int) {
        if (right <= left || bottom <= top) return
        paint.color = color
        canvas.drawRect(left, top, right, bottom, paint)
    }

    private companion object {
        const val COLOR_WHITE = 0xFFFFFFFF.toInt()
        const val COLOR_LIGHT = 0xFFDFDFDF.toInt()
        const val COLOR_FACE = 0xFFBDBDBD.toInt()
        const val COLOR_OPEN_FACE = 0xFFC0C0C0.toInt()
        const val COLOR_GRID = 0xFF808080.toInt()
        const val COLOR_DARK = 0xFF707070.toInt()

    }
}
