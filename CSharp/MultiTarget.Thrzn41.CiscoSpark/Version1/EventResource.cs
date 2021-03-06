﻿/* 
 * MIT License
 * 
 * Copyright(c) 2017 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Event resouce of Cisco Spark.
    /// </summary>
    public class EventResource
    {

        /// <summary>
        /// All resouces.
        /// </summary>
        public static readonly EventResource All = new EventResource("all");

        /// <summary>
        /// Space membership resouce.
        /// </summary>
        public static readonly EventResource SpaceMembership = new EventResource("memberships");

        /// <summary>
        /// Message resouce.
        /// </summary>
        public static readonly EventResource Message = new EventResource("messages");

        /// <summary>
        /// Space resouce.
        /// </summary>
        public static readonly EventResource Space = new EventResource("rooms");




        /// <summary>
        /// Dictionary for event resouce.
        /// </summary>
        private static readonly Dictionary<string, EventResource> EVENT_RESOUCES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static EventResource()
        {
            EVENT_RESOUCES = new Dictionary<string, EventResource>();

            EVENT_RESOUCES.Add(All.Name,             All);
            EVENT_RESOUCES.Add(SpaceMembership.Name, SpaceMembership);
            EVENT_RESOUCES.Add(Message.Name,         Message);
            EVENT_RESOUCES.Add(Space.Name,           Space);
        }


        /// <summary>
        /// Name of the event resouce.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the event resouce.</param>
        private EventResource(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse event resouce.
        /// </summary>
        /// <param name="name">Name of the event resouce.</param>
        /// <returns><see cref="EventResource"/> for the name.</returns>
        public static EventResource Parse(string name)
        {
            EventResource eventResource = null;

            if ( name == null || !EVENT_RESOUCES.TryGetValue(name, out eventResource) )
            {
                eventResource = new EventResource(name);
            }

            return eventResource;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="EventResource"/> object have the same value.
        /// </summary>
        /// <param name="value">The event resouce to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(EventResource value)
        {
            if ( Object.ReferenceEquals(value, null) )
            {
                return false;
            }

            if ( Object.ReferenceEquals(this, value) )
            {
                return true;
            }

            return (this.Name == value.Name);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="EventResource"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The event resouce to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="EventResource"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as EventResource);
        }

        /// <summary>
        /// Returns the hash code for this event resouce.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified event resouce have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(EventResource lhs, EventResource rhs)
        {
            if ( Object.ReferenceEquals(lhs, null) )
            {
                if ( Object.ReferenceEquals(rhs, null) )
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two specified event resouce have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(EventResource lhs, EventResource rhs)
        {
            return !(lhs == rhs);
        }

    }

}
