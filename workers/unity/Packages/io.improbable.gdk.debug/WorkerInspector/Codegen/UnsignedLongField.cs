using System;
using System.Globalization;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Improbable.Gdk.Debug.WorkerInspector.Codegen
{
    public class UnsignedLongField : TextValueField<ulong>
    {
        /// <summary>
        ///   <para>USS class name of elements of this type.</para>
        /// </summary>
        public new static readonly string ussClassName = "unity-integer-field";

        /// <summary>
        ///   <para>USS class name of labels in elements of this type.</para>
        /// </summary>
        public new static readonly string labelUssClassName = IntegerField.ussClassName + "__label";

        /// <summary>
        ///   <para>USS class name of input elements in elements of this type.</para>
        /// </summary>
        public new static readonly string inputUssClassName = IntegerField.ussClassName + "__input";

        private UnsignedLongInput integerInput => (UnsignedLongInput) textInputBase;

        /// <summary>
        ///   <para>Converts the given integer to a string.</para>
        /// </summary>
        /// <param name="v">The integer to be converted to string.</param>
        /// <returns>
        ///   <para>The integer as string.</para>
        /// </returns>
        protected override string ValueToString(ulong v) => v.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);

        /// <summary>
        ///   <para>Converts a string to an integer.</para>
        /// </summary>
        /// <param name="str">The string to convert.</param>
        /// <returns>
        ///   <para>The integer parsed from the string.</para>
        /// </returns>
        protected override ulong StringToValue(string str)
        {
            ExpressionEvaluator.Evaluate(str, out ulong num);
            return num;
        }

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        /// <param name="maxLength">Maximum number of characters the field can take.</param>
        public UnsignedLongField()
            : this(null)
        {
        }

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        /// <param name="maxLength">Maximum number of characters the field can take.</param>
        public UnsignedLongField(int maxLength)
            : this(null, maxLength)
        {
        }

        public UnsignedLongField(string label, int maxLength = -1)
            : base(label, maxLength, new UnsignedLongInput())
        {
            AddLabelDragger<ulong>();
        }

        /// <summary>
        ///   <para>Modify the value using a 3D delta and a speed, typically coming from an input device.</para>
        /// </summary>
        /// <param name="delta">A vector used to compute the value change.</param>
        /// <param name="speed">A multiplier for the value change.</param>
        /// <param name="startValue">The start value.</param>
        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, ulong startValue) =>
            integerInput.ApplyInputDeviceDelta(delta, speed, startValue);

        /// <summary>
        ///   <para>Instantiates an IntegerField using the data read from a UXML file.</para>
        /// </summary>
        public new class UxmlFactory : UxmlFactory<UnsignedLongField, UxmlTraits>
        {
        }

        /// <summary>
        ///   <para>Defines UxmlTraits for the IntegerField.</para>
        /// </summary>
        public new class UxmlTraits : TextValueFieldTraits<int, UxmlIntAttributeDescription>
        {
        }

        private class UnsignedLongInput : TextValueInput
        {
            private UnsignedLongField parentIntegerField => (UnsignedLongField) parent;

            internal UnsignedLongInput()
            {
                formatString = "#######0"; // From: `EditorGUI.kIntFieldFormatString`
            }

            protected override string allowedCharacters => "0123456789-*/+%^()";

            public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, ulong startValue)
            {
                var intDragSensitivity = NumericFieldDraggerUtility.CalculateIntDragSensitivity((long) startValue);
                var acceleration =
                    NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
                var num = StringToValue(text) + (ulong) Math.Round((double) NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * intDragSensitivity);
                if (parentIntegerField.isDelayed)
                {
                    text = ValueToString(num);
                }
                else
                {
                    parentIntegerField.value = num;
                }
            }

            protected override string ValueToString(ulong v) => v.ToString(formatString);

            protected override ulong StringToValue(string str)
            {
                ExpressionEvaluator.Evaluate(str, out ulong num);
                return num;
            }
        }
    }
}