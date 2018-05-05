using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace QuickBuild
{

    public class QBMessagePacker
    {
        private const char HeaderSeparator = ':';
        
        public static string	PackMessage(string condition, string stackTrace, LogType type)
        {
            string header = PackHeader(condition, stackTrace);
            return (string.Format("{0}{1}{2}{3}", header, condition, stackTrace, type));
        }

        public static bool		UnpackMessage(string packedMessage, out string condition, out string stackTrace, out LogType logType)
        {
            int conditionLength;
            int stackTraceLength;
            int messageStartIndex;
            if (!ExtractHeader(packedMessage, out conditionLength, out stackTraceLength, out messageStartIndex))
            {
                condition = stackTrace = string.Empty;
                logType = LogType.Assert;
                return (false);
            }

            condition = packedMessage.Substring(messageStartIndex, conditionLength);
            stackTrace = packedMessage.Substring(messageStartIndex + conditionLength, stackTraceLength);

            object logTypeRaw = Enum.Parse(typeof(LogType), packedMessage.Substring(messageStartIndex + conditionLength + stackTraceLength));
            if (logTypeRaw == null)
            {
                condition = stackTrace = string.Empty;
                logType = LogType.Assert;
                return (false);
            }

            logType = (LogType)logTypeRaw;
            return (true);
        }

        static string PackHeader(string condition, string stackTrace)
        {
            return (string.Format("{1}{0}{2}{0}", HeaderSeparator, condition.Length, stackTrace.Length));
        }

        static bool ExtractHeader(string message, out int conditionLength, out int stackTraceLength, out int messageStartIndex)
        {
            conditionLength = stackTraceLength = messageStartIndex = 0;

            string[] messageCuts = message.Split(new char[] { HeaderSeparator }, StringSplitOptions.None);
            if (messageCuts.Length < 2 ||
                !int.TryParse(messageCuts[0], out conditionLength) ||
                !int.TryParse(messageCuts[1], out stackTraceLength))
            {
                return (false);
            }

            messageStartIndex = messageCuts[0].Length + messageCuts[1].Length + 2; // 2 for the 2 separators
            return (true);
        }
    }

}