using System;
using System.Reflection;
using IniParser;
using IniParser.Model;

namespace SodikmLauncher;

internal class IniParser<T> where T : new()
{
	private static FileIniDataParser IniDataParser = new FileIniDataParser();

	public static T ParseFromFile(string fileName)
	{
		IniData iniData = IniDataParser.ReadFile(fileName);
		T val = new T();
		PropertyInfo[] array = val?.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in array)
		{
			IniVariableAttribute iniVariableAttribute = (IniVariableAttribute)propertyInfo.GetCustomAttribute(typeof(IniVariableAttribute), inherit: false);
			if (iniVariableAttribute != null && iniData.TryGetKey(iniVariableAttribute.Name, out var value))
			{
				propertyInfo.SetValue(val, Convert.ChangeType(value, propertyInfo.PropertyType));
			}
		}
		return val;
	}

	public static bool TryParseFromFile(string fileName, out T ini)
	{
		try
		{
			ini = ParseFromFile(fileName);
		}
		catch (Exception)
		{
			ini = default(T);
			return false;
		}
		return true;
	}
}
