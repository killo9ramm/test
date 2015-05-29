using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace RBClient.WPF.UserControls
{
    class MyPanel : Panel
    {
        // Измерение
        //
        // На входе - сколько есть у родителя для менеджера
        // На выходе - сколько просит менеджер для себя
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxChildWidth = 0.0;
            double maxChildHeight = 0.0;

            // Измерить требования каждого потомка и определить самые большие
            foreach (UIElement child in this.InternalChildren)
            {
                //child.Measure(new Size(availableSize.Width/this.InternalChildren.Count,availableSize.Height));
                child.Measure(availableSize);
                maxChildWidth = Math.Max(child.DesiredSize.Width, maxChildWidth);
                maxChildHeight = Math.Max(child.DesiredSize.Height, maxChildHeight);
            }

            // Приблизительный несовершенный алгоритм
            //
            // Требуемая для размещения всех элементов длина окружности

            double minimalLineLength =
                maxChildWidth * this.InternalChildren.Count;

            // Необходимые размеры описывающего окружность квадрата
            Size minialLineSquare = new Size(minimalLineLength, maxChildHeight);

            //можно добавит логику просчета на сколько увеличить кнопки

            Size desired = minialLineSquare;// Менеджер столько хочет для себя
            // Если выделяемый менеджеру размер небесконечен

            if (!double.IsInfinity(availableSize.Width))
            {
                // Если требуемого размера менеджеру нехватает
                if (availableSize.Width < desired.Width)
                {
                    // Корректируем размер менеджера
                    desired.Width = availableSize.Width;
                }
            }
            // То же самое и для высоты
            if (!double.IsInfinity(availableSize.Height))
            {
                if (availableSize.Height < desired.Height)
                {
                    desired.Height = availableSize.Height;
                }
            }

            return desired;
        }

        // Установка (заключение контракта)
        //
        // Делим отведенную для менеджера площадь
        static double leftXX = 0.0;
        static double topYY = 0.0;
        public double margin = 10.0;

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Размещаем квадратный менеджер в
            // центре выделенной родителем области
            Rect layoutRect = new Rect(finalSize);
            leftXX = 0.0;
            topYY = layoutRect.Top;

            // Расставляем по кругу все дочерние элементы
            foreach (UIElement child in this.InternalChildren)
            {
                // Располагаем очередной элемент в верхней точке круга

                if ((leftXX + child.DesiredSize.Width + margin) > finalSize.Width)
                {
                    topYY += child.DesiredSize.Height + margin;
                    leftXX = 0.0;
                }

                Point childLocation = new Point(
                    layoutRect.Left + leftXX,
                    topYY);

                leftXX += child.DesiredSize.Width + margin;

                // Переместили по часовой стрелке и повернули вокруг оси
                //child.RenderTransform = new RotateTransform(
                //    angle,
                //    child.DesiredSize.Width / 2,
                //    finalSize.Height / 2 - layoutRect.Top);

                // Утвердили окончательный размер и положение потомка
                child.Arrange(new Rect(childLocation, child.DesiredSize));

                //angle += angleInc;
            }

            return base.ArrangeOverride(finalSize);
        }
    }

}
